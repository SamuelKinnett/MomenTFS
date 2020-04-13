using Eto.Drawing;
using MomenTFS.Extensions;
using MomenTFS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MomenTFS.Reader
{
    public class TFSReader
    {
        private const int TILE_WIDTH = 128;
        private const int TILE_HEIGHT = 128;

        public int PaletteCount { get => paletteInfo.ClutNum; }
        public bool ImageLoaded { get; private set; }

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
            var red = bytes & 0x1F;
            var green = (bytes >> 5) & 0x1F;
            var blue = (bytes >> 10) & 0x1F;
            return new Color(red / 32f, green / 32f, blue / 32f);
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

            List<Color> bitmapDataList = new List<Color>();
            Color fallbackColor = new Color(0, 0, 0);
            for (var y = 0; y < bitmapHeight; ++y) {
                for (var x = 0; x < bitmapWidth; ++x) {
                    if (bitmapData.ContainsKey(x) && bitmapData[x].ContainsKey(y)) {
                        int colorNumber = bitmapData[x][y];
                        bitmapDataList.Add(colorLookupTable[paletteIndex, colorNumber]);
                    } else {
                        bitmapDataList.Add(fallbackColor);
                    }
                }
            }

            return new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb, bitmapDataList);
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
                var currentColorWord = stream.ReadShort();
                colorLookupTableData.Add(currentColorWord);
            }

            populateColourLookupTable(colorLookupTableData);

            for (var tileIndex = 0; tileIndex < (header.Width * header.Height); ++tileIndex) {
                var tileData = new List<byte>();
                int tileX = stream.ReadShort() * 2;
                int tileY = stream.ReadShort();

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
