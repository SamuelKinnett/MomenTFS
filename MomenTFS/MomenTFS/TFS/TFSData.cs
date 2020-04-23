using MomenTFS.Objects;
using MomenTFS.TFS.Objects;
using MomenTFS.TIM;
using System.Drawing;

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

        public Color[,] GetBitmap(int paletteIndex) {
            int bitmapWidth = ImageSize.X;
            int bitmapHeight = ImageSize.Y;

            var fallbackColor = Color.FromArgb(0, 0, 0, 0);
            var bitmap = new Color[bitmapWidth, bitmapHeight];

            for (var y = 0; y < bitmapHeight; ++y) {
                for (var x = 0; x < bitmapWidth; ++x) {
                    var index = ImageData[x, y];
                    var color = index > -1
                        ? ColourLookupTable.LookupTable[index, paletteIndex]
                            .GetAsSystemColor()
                        : fallbackColor;

                    bitmap[x, y] = color;
                }
            }

            return bitmap;
        }

        public Color[] GetBitmapAsFlatArray(int paletteIndex) {
            int bitmapWidth = ImageSize.X;
            int bitmapHeight = ImageSize.Y;

            var fallbackColor = Color.FromArgb(0, 0, 0, 0);
            var bitmap = new Color[bitmapWidth * bitmapHeight];

            for (var y = 0; y < bitmapHeight; ++y) {
                for (var x = 0; x < bitmapWidth; ++x) {
                    var index = ImageData[x, y];
                    var color = index > -1
                        ? ColourLookupTable.LookupTable[index, paletteIndex]
                            .GetAsSystemColor()
                        : fallbackColor;

                    bitmap[y * bitmapWidth + x] = color;
                }
            }

            return bitmap;
        }
    }
}
