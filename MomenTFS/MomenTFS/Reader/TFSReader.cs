using MomenTFS.Extensions;
using MomenTFS.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace MomenTFS.Reader
{
    public class TFSReader
    {
        private const int TILE_WIDTH = 128;
        private const int TILE_HEIGHT = 128;

        // Maximum values had to be added in due to one map file (MAYO10.TFS) having a tile 
        // with an X co-ordinate of 131,070
        private const int MAX_WIDTH = 4096;
        private const int MAX_HEIGHT = 4096;

        public int PaletteCount { get => paletteInfo.ClutNum; }
        public bool ImageLoaded { get; private set; }
        public IVector3 ImageSize { get {
                int bitmapWidth = bitmapData.Keys.Max() + 1;
                int bitmapHeight = bitmapData[0].Keys.Max() + 1;

                return new IVector3(bitmapWidth, bitmapHeight, 0);
            }
        }

        private TFSHeader header;
        private PaletteInfo paletteInfo;
        private Color[,] colorLookupTable;
        private Dictionary<int, Dictionary<int, int>> bitmapData;

        public TFSReader() {
            paletteInfo = new PaletteInfo();
            ImageLoaded = false;
        }

        // Converts a 15bpp color to an Eto color
        protected Color ShortToColor(ushort bytes) {
            var red = (bytes & 0x1F) * 8;
            var green = ((bytes >> 5) & 0x1F) * 8;
            var blue = ((bytes >> 10) & 0x1F) * 8;
            return Color.FromArgb(red, green, blue);
        }

        protected void populateColourLookupTable(List<ushort> colourLookupTableData) {
            var dataIndex = 0;
            var width = paletteInfo.ClutColors;
            var height = paletteInfo.ClutNum;
            colorLookupTable = new Color[height, width];

            for (var x = 0; x < height; ++x) {
                for (var y = 0; y < width; ++y) {
                    colorLookupTable[x, y] = ShortToColor(colourLookupTableData[dataIndex]);
                    ++dataIndex;
                }
            }
        }

        public Bitmap RenderImage(int paletteIndex) {
            if (!ImageLoaded) {
                throw new Exception("Can't render image until Read() has been called");
            }

            int bitmapWidth = bitmapData.Keys.Max() + 1;
            int bitmapHeight = bitmapData[0].Keys.Max() + 1;

            Color fallbackColor = Color.FromArgb(0, 0, 0);
            Bitmap renderedBitmap
                = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb);

            try {
                var renderedBitmapData = renderedBitmap.LockBits(
                    new Rectangle(0, 0, bitmapWidth, bitmapHeight),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppRgb);
                var stride = renderedBitmapData.Stride;

                unsafe {
                    byte* bitmapPointer = (byte*)renderedBitmapData.Scan0;

                    for (var y = 0; y < bitmapHeight; ++y) {
                        for (var x = 0; x < bitmapWidth; ++x) {
                            Color pixelColor = fallbackColor;

                            if (bitmapData.ContainsKey(x) && bitmapData[x].ContainsKey(y)) {
                                int colorNumber = bitmapData[x][y];
                                pixelColor = colorLookupTable[paletteIndex, colorNumber];
                            }

                            bitmapPointer[(x * 4) + (y * stride)] = pixelColor.B;
                            bitmapPointer[(x * 4) + (y * stride) + 1] = pixelColor.G;
                            bitmapPointer[(x * 4) + (y * stride) + 2] = pixelColor.R;
                            bitmapPointer[(x * 4) + (y * stride) + 3] = 0x00;
                        }
                    }
                }

                renderedBitmap.UnlockBits(renderedBitmapData);
            } catch (Exception ex) {

            }

            return renderedBitmap;
        }


        public void Read(string filename) {
            using (var fileStream = new FileStream(filename, FileMode.Open)) {
                Read(fileStream);
            }
        }

        public void Read(Stream stream) {
            bitmapData = new Dictionary<int, Dictionary<int, int>>();

            header = new TFSHeader(stream);

            paletteInfo.ClutColors = 256;
            paletteInfo.ClutNum = header.PaletteCount;

            var colorLookupTableData = new List<ushort>();

            for (var i = 0; i < paletteInfo.ClutColors * paletteInfo.ClutNum; ++i) {
                var currentColorWord = stream.ReadUShort();
                colorLookupTableData.Add(currentColorWord);
            }

            populateColourLookupTable(colorLookupTableData);

            for (var tileIndex = 0; tileIndex < (header.Width * header.Height); ++tileIndex) {
                var tileData = new List<byte>();
                int tileX = stream.ReadUShort() * 2;
                int tileY = stream.ReadUShort();

                if (tileX + TILE_WIDTH > MAX_WIDTH) {
                    tileX = MAX_WIDTH - TILE_WIDTH;
                }

                if (tileY + TILE_HEIGHT > MAX_HEIGHT) {
                    tileY = MAX_HEIGHT - TILE_HEIGHT;
                }

                for (var i = 0; i < (TILE_WIDTH * TILE_HEIGHT); ++i) {
                    tileData.Add((byte)stream.ReadByte());
                }

                var tileDataIndex = 0;

                for (var y = 0; y < TILE_HEIGHT; ++y) {
                    for (var x = 0; x < TILE_WIDTH; ++x) {
                        if (!bitmapData.ContainsKey(x + tileX)) {
                            bitmapData[x + tileX] = new Dictionary<int, int>();
                        }

                        bitmapData[x + tileX][y + tileY] = tileData[tileDataIndex];
                        ++tileDataIndex;
                    }
                }
            }

            ImageLoaded = true;
        }
    }
}
