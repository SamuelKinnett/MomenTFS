using MomenTFS.Extensions;
using MomenTFS.MAP.Enums;
using MomenTFS.MAP.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.Settings
{
    public class MAPSettings
    {
        public IntVector3 CameraOrigin { get; private set; }
        public IntVector3 CameraTranslation { get; private set; }
        public List<Light> Lights { get; private set; }
        public ushort Zoom { get; private set; }
        public ushort SpriteScale { get; private set; }
        public int[] AreaLikeTypes { get; private set; }
        public int[] AreaDislikeTypes { get; private set; }
        public int MapTileWidth { get; private set; }
        public int MapTileHeight { get; private set; }
        public int[,] MapTiles { get; private set; }

        public MAPSettings(Stream stream) {
            CameraOrigin = stream.ReadIntVector3();
            CameraTranslation = stream.ReadIntVector3();

            Lights = new List<Light>();
            for (int i = 0; i < 3; ++i) {
                var lightPosition = stream.ReadIntVector3();
                var lightColor = stream.ReadIntVector3();

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
