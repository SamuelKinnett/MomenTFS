using MomenTFS.Objects;
using MomenTFS.TFS.Objects;
using MomenTFS.TIM;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MomenTFS.TFS
{
    public class TFSData
    {
        public TFSHeader Header { get; set; }
        public CLUT ColourLookupTable { get; set; }
        public short[,] ImageData { get; set; }

        public int PaletteCount { get => ColourLookupTable.Height; }
        public IVector2 ImageSize {
            get {
                return new IVector2(ImageData.GetLength(0), ImageData.GetLength(1));
            }
        }

        public Bitmap ToBitmap(int paletteIndex) {
            int bitmapWidth = ImageSize.X;
            int bitmapHeight = ImageSize.Y;

            Color fallbackColor = Color.FromArgb(0, 0, 0, 0);
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
                            var index = ImageData[x, y];
                            var color = index > -1
                                ? ColourLookupTable.LookupTable[index, paletteIndex]
                                    .GetAsSystemColor()
                                : fallbackColor;

                            bitmapPointer[(x * 4) + (y * stride)] = color.B;
                            bitmapPointer[(x * 4) + (y * stride) + 1] = color.G;
                            bitmapPointer[(x * 4) + (y * stride) + 2] = color.R;
                            bitmapPointer[(x * 4) + (y * stride) + 3] = color.A;
                        }
                    }
                }

                renderedBitmap.UnlockBits(renderedBitmapData);
            } catch (Exception ex) {

            }

            return renderedBitmap;
        }
    }
}
