using MomenTFS.Extensions;
using MomenTFS.Objects;
using MomenTFS.TFS.Objects;
using MomenTFS.TIM;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MomenTFS.TFS
{
    public class TFSReader : TFSBase {

        public IVector3 ImageSize {
            get {
                int bitmapWidth = bitmapData.Keys.Max() + 1;
                int bitmapHeight = bitmapData[0].Keys.Max() + 1;

                return new IVector3(bitmapWidth, bitmapHeight, 0);
            }
        }

        private Dictionary<int, Dictionary<int, int>> bitmapData;

        public TFSData Read(string filename) {
            using (var fileStream = new FileStream(filename, FileMode.Open)) {
                return Read(fileStream);
            }
        }

        public TFSData Read(Stream stream) {
            TFSData tfsData = new TFSData();

            bitmapData = new Dictionary<int, Dictionary<int, int>>();

            tfsData.Header = new TFSHeader(stream);

            CLUT colorLookupTable = new CLUT();
            colorLookupTable.Width = 256;
            colorLookupTable.Height = tfsData.Header.PaletteCount;

            var newLookupTable = new CLUTColor[colorLookupTable.Width, colorLookupTable.Height];
            for (var y = 0; y < colorLookupTable.Height; ++y) {
                for (var x = 0; x < colorLookupTable.Width; ++x) {
                    var data = stream.ReadUShort();
                    newLookupTable[x, y] = new CLUTColor(data);
                }
            }

            colorLookupTable.LookupTable = newLookupTable;
            tfsData.ColourLookupTable = colorLookupTable;

            for (var tileIndex = 0;
                    tileIndex < (tfsData.Header.Width * tfsData.Header.Height);
                    ++tileIndex) {
                var tileData = new List<byte>();
                int tileX = stream.ReadShort() * 2;
                int tileY = stream.ReadShort();

                if (tileX < 0 || tileY < 0) {
                    continue;
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

            int imageDataWidth = bitmapData.Keys.Max() + 1;
            int imageDataHeight = bitmapData[0].Keys.Max() + 1;

            short[,] imageData = new short[imageDataWidth, imageDataHeight];

            for (int y = 0; y < imageDataHeight; ++y) {
                for (int x = 0; x < imageDataWidth; ++x) {
                    var currentIndex = -1;

                    if (bitmapData.ContainsKey(x) && bitmapData[x].ContainsKey(y)) {
                        currentIndex = bitmapData[x][y];
                    }

                    imageData[x, y] = (byte)currentIndex;
                }
            }

            tfsData.ImageData = imageData;

            return tfsData;
        }
    }
}
