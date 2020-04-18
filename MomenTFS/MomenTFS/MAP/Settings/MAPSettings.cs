using MomenTFS.Extensions;
using MomenTFS.Objects;
using System.Collections.Generic;
using System.IO;

namespace MomenTFS.MAP.Settings
{
    public class MAPSettings
    {
        public IVector3 CameraOrigin { get; private set; }
        public IVector3 CameraTranslation { get; private set; }
        public List<Light> Lights { get; private set; }
        public ushort Zoom { get; private set; }
        public ushort SpriteScale { get; private set; }
        public int[] AreaLikeTypes { get; private set; }
        public int[] AreaDislikeTypes { get; private set; }
        public int MapTileWidth { get; private set; }
        public int MapTileHeight { get; private set; }
        public int[,] MapTiles { get; private set; }

        public MAPSettings(Stream stream) {
            CameraOrigin = stream.ReadIVector3();
            CameraTranslation = stream.ReadIVector3();

            Lights = new List<Light>();
            for (int i = 0; i < 3; ++i) {
                var lightPosition = stream.ReadIVector3();
                var lightColor = stream.ReadIVector3();

                Lights.Add(new Light(lightPosition, lightColor));
            }

            stream.Seek(12, SeekOrigin.Current);

            Zoom = stream.ReadUShort();
            SpriteScale = stream.ReadUShort();

            AreaLikeTypes = new int[4];
            AreaLikeTypes[0] = stream.ReadInt();
            AreaLikeTypes[1] = stream.ReadInt();
            AreaLikeTypes[2] = stream.ReadInt();
            AreaLikeTypes[3] = stream.ReadInt();

            AreaDislikeTypes = new int[4];
            AreaDislikeTypes[0] = stream.ReadInt();
            AreaDislikeTypes[1] = stream.ReadInt();
            AreaDislikeTypes[2] = stream.ReadInt();
            AreaDislikeTypes[3] = stream.ReadInt();

            MapTileWidth = stream.ReadInt();
            MapTileHeight = stream.ReadInt();
            MapTiles = new int[MapTileWidth, MapTileHeight];

            for (int y = 0; y < MapTileHeight; ++y) {
                for (int x = 0; x < MapTileWidth; ++x) {
                    MapTiles[x, y] = stream.ReadInt();
                }
            }
        }
    }
}
