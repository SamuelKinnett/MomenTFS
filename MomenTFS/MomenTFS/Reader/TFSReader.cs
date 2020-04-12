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

        private int paletteIndex = 0;

        private TFSHeader header;
        private PaletteInfo palInfo;
        private ImageInfo imageInfo;
        private Color[,] clutMas;
        private List<ushort> clutData;
        private List<ushort> imageData;
        private bool cls = false;
        private Bitmap bitmap;
        private byte zoomValue = 10;
        private bool fastPaint = false;

        public TFSReader() {
            palInfo = new PaletteInfo();
            imageInfo = new ImageInfo();
            clutData = new List<ushort>();
            imageData = new List<ushort>();
        }

        // Converts a 15bpp color to an Eto color
        protected Color ShortToColor(ushort bytes) {
            var red = (bytes & 0x1F) * 8;
            var green = ((bytes >> 5) & 0x1F) * 8;
            var blue = ((bytes >> 10) & 0x1F) * 8;
            return new Color(red / 255f, green / 255f, blue / 255f);
        }

        protected void paintClut() {
            var dataIndex = 0;
            var width = palInfo.ClutColors;
            var height = palInfo.ClutNum;
            clutMas = new Color[height, width];

            for (var x = 0; x < height; ++x) {
                for (var y = 0; y < width; ++y) {
                    clutMas[x, y] = ShortToColor(clutData[dataIndex]);
                    ++dataIndex;
                }
            }
        }

        public Bitmap Read(string filename) {
            Dictionary<int, Dictionary<int, Color>> bitmapData = new Dictionary<int, Dictionary<int, Color>>();

            using (var fileStream = new FileStream(filename, FileMode.Open)) {
                header = new TFSHeader(fileStream);

                palInfo.ClutColors = 0x100;
                palInfo.ClutNum = header.PalNum;

                for (var i = 0; i < palInfo.ClutColors * palInfo.ClutNum; ++i) {
                    var data = fileStream.ReadShort();
                    clutData.Add(data);
                }

                paintClut();

                var k = 0;
                var row = 0;
                var column = 0;

                for (var j = 0; j < (header.Width * header.Height); ++j) {
                    imageData.Clear();
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
                                    Color color = clutMas[paletteIndex, imageData[z]];
                                    //bitmap.SetPixel(x, y, color);
                                    if (!bitmapData.ContainsKey(x)) {
                                        bitmapData[x] = new Dictionary<int, Color>();
                                    }

                                    bitmapData[x][y] = color;
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
                                    Color color = clutMas[paletteIndex, imageData[z]];
                                    //bitmap.SetPixel(x + imageInfo.ImageX, y + imageInfo.ImageY, color);
                                    if (!bitmapData.ContainsKey(x + imageInfo.ImageX)) {
                                        bitmapData[x + imageInfo.ImageX] = new Dictionary<int, Color>();
                                    }

                                    bitmapData[x + imageInfo.ImageX][y + imageInfo.ImageY] = color;
                                    ++z;
                                }
                            }
                        }
                        row = (int)Math.Truncate(bitmapData.Keys.Max() / (double)IMAGE_REAL_WIDTH);
                        // Draw bitmap chunk
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
                                    var color = clutMas[paletteIndex, imageData[z]];
                                    if (!bitmapData.ContainsKey(x + row * IMAGE_REAL_WIDTH)) {
                                        bitmapData[x + row * IMAGE_REAL_WIDTH] = new Dictionary<int, Color>();
                                    }

                                    bitmapData[x + row * IMAGE_REAL_WIDTH][y + column * IMAGE_REAL_HEIGHT] = color;
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
                                    var color = clutMas[paletteIndex, imageData[z]];
                                    if (!bitmapData.ContainsKey(x + imageInfo.ImageX)) {
                                        bitmapData[x + imageInfo.ImageX] = new Dictionary<int, Color>();
                                    }

                                    bitmapData[x + imageInfo.ImageX][y + imageInfo.ImageY] = color;
                                    ++z;
                                }
                            }
                        }
                        // Draw bitmap chunk
                    }

                    ++k;
                }
            }

            int bitmapWidth = bitmapData.Keys.Max();
            int bitmapHeight = bitmapData[0].Keys.Max();


            List<Color> bitmapDataList = new List<Color>();
            for (var y = 0; y < bitmapHeight; ++y) {
                for (var x = 0; x < bitmapWidth; ++x) {
                    bitmapDataList.Add(bitmapData[x][y]);
                }
            }

            bitmap = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format32bppRgb, bitmapDataList);

            return bitmap;
        }
    }
}
