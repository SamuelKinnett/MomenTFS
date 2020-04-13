using Eto.Drawing;
using Eto.Forms;
using MomenTFS.Extensions;
using MomenTFS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MomenTFS.Reader
{
    public class TFSReader
    {
        private const int IMAGE_REAL_WIDTH = 128;
        private const int IMAGE_REAL_HEIGHT = 128;

        private TFSHeader header;
        private PaletteInfo paletteInfo;
        private ImageInfo imageInfo;
        private Color[,] colorLookupTable;
        private Boolean imageLoaded = false;
        private Dictionary<int, Dictionary<int, int>> bitmapData;

        public TFSReader() {
            paletteInfo = new PaletteInfo();
            imageInfo = new ImageInfo();
        }

        public int GetPaletteCount() {
            return paletteInfo.ClutNum;
        }

        public bool GetLoaded() {
            return imageLoaded;
        }

        // Converts a 15bpp color to an Eto color
        protected Color ShortToColor(ushort bytes) {
            var red = (bytes & 0x1F) * 8;
            var green = ((bytes >> 5) & 0x1F) * 8;
            var blue = ((bytes >> 10) & 0x1F) * 8;
            return new Color(red / 255f, green / 255f, blue / 255f);
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
            if (!imageLoaded) {
                throw new Exception("Can't render image until Read() has been called");
            }

            int bitmapWidth = bitmapData.Keys.Max();
            int bitmapHeight = bitmapData[0].Keys.Max();

            List<Color> bitmapDataList = new List<Color>();
            for (var y = 0; y < bitmapHeight; ++y) {
                for (var x = 0; x < bitmapWidth; ++x) {
                    int colorNumber = bitmapData[x][y];
                    bitmapDataList.Add(colorLookupTable[paletteIndex, colorNumber]);
                }
            }

            return new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb, bitmapDataList);
        }

        public void Read(string filename) {
            bitmapData = new Dictionary<int, Dictionary<int, int>>();

            using (var fileStream = new FileStream(filename, FileMode.Open)) {
                header = new TFSHeader(fileStream);

                paletteInfo.ClutColors = 0x100;
                paletteInfo.ClutNum = header.PalNum;

                var colourLookupTableData = new List<ushort>();

                for (var i = 0; i < paletteInfo.ClutColors * paletteInfo.ClutNum; ++i) {
                    var data = fileStream.ReadShort();
                    colourLookupTableData.Add(data);
                }

                populateColourLookupTable(colourLookupTableData);

                var k = 0;
                var row = 0;
                var column = 0;

                for (var j = 0; j < (header.Width * header.Height); ++j) {
                    var imageData = new List<ushort>();
                    imageInfo.ImageX = fileStream.ReadShort() * 2;
                    imageInfo.ImageY = fileStream.ReadShort();

                    for (var i = 0; i < (IMAGE_REAL_WIDTH * IMAGE_REAL_HEIGHT); ++i) {
                        imageData.Add((byte)fileStream.ReadByte());
                    }

                    var z = 0;

                    if (k == 0) {
                        if (imageInfo.ImageX == 0 && imageInfo.ImageY == 0) {
                            //bitmap = new Bitmap(IMAGE_REAL_WIDTH, IMAGE_REAL_HEIGHT, PixelFormat.Format32bppRgb);
                            for (var y = 0; y < IMAGE_REAL_HEIGHT; ++y) {
                                for (var x = 0; x < IMAGE_REAL_WIDTH; ++x) {
                                    if (!bitmapData.ContainsKey(x)) {
                                        bitmapData[x] = new Dictionary<int, int>();
                                    }

                                    bitmapData[x][y] = imageData[z];
                                    ++z;
                                }
                            }
                        } else if (imageInfo.ImageX > 0 || imageInfo.ImageY > 0) {
                            //bitmap = new Bitmap(
                            //    header.Width * IMAGE_REAL_WIDTH,
                            //    header.Height * IMAGE_REAL_HEIGHT,
                            //    PixelFormat.Format32bppRgb);
                            for (var y = 0; y < IMAGE_REAL_HEIGHT; ++y) {
                                for (var x = 0; x < IMAGE_REAL_WIDTH; ++x) {
                                    //bitmap.SetPixel(x + imageInfo.ImageX, y + imageInfo.ImageY, color);
                                    if (!bitmapData.ContainsKey(x + imageInfo.ImageX)) {
                                        bitmapData[x + imageInfo.ImageX] = new Dictionary<int, int>();
                                    }

                                    bitmapData[x + imageInfo.ImageX][y + imageInfo.ImageY] = imageData[z];
                                    ++z;
                                }
                            }
                        }
                        row = (int)Math.Truncate(bitmapData.Keys.Max() / (double)IMAGE_REAL_WIDTH);
                    } else {
                        if (imageInfo.ImageX == 0 && imageInfo.ImageY == 0) {
                            if (row == header.Width || row + 1 == header.Width) {
                                row = 0;
                                ++column;
                            } else {
                                ++row;
                            }

                            for (var y = 0; y < IMAGE_REAL_HEIGHT; ++y) {
                                for (var x = 0; x < IMAGE_REAL_WIDTH; ++x) {
                                    if (!bitmapData.ContainsKey(x + row * IMAGE_REAL_WIDTH)) {
                                        bitmapData[x + row * IMAGE_REAL_WIDTH] = new Dictionary<int, int>();
                                    }

                                    bitmapData[x + row * IMAGE_REAL_WIDTH][y + column * IMAGE_REAL_HEIGHT] = imageData[z];
                                    ++z;
                                }
                            }

                            if (row < header.Width) {
                                ++row;
                                if (column != 0) {
                                    --row;
                                }
                            }
                        } else if (imageInfo.ImageX > 0 || imageInfo.ImageY > 0) {
                            for (var y = 0; y < IMAGE_REAL_HEIGHT; ++y) {
                                for (var x = 0; x < IMAGE_REAL_WIDTH; ++x) {
                                    if (!bitmapData.ContainsKey(x + imageInfo.ImageX)) {
                                        bitmapData[x + imageInfo.ImageX] = new Dictionary<int, int>();
                                    }

                                    bitmapData[x + imageInfo.ImageX][y + imageInfo.ImageY] = imageData[z];
                                    ++z;
                                }
                            }
                        }
                    }

                    ++k;
                }
            }

            imageLoaded = true;
        }
    }
}
