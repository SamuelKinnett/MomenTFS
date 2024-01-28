using MomenTFS.Extensions;
using MomenTFS.Objects;
using System.Collections.Generic;
using System.IO;

namespace MomenTFS.MAP.Settings
{
    public class MAPSettings
    {
        public IVector3 CameraOrigin { get; set; }
        public IVector3 CameraTranslation { get; set; }
        public Light[] Lights { get; set; }
        public ushort Zoom { get; set; }
        public ushort SpriteScale { get; set; }
        public int[] AreaLikeTypes { get; set; }
        public int[] AreaDislikeTypes { get; set; }
        public int MapTileWidth { get; set; }
        public int MapTileHeight { get; set; }
        public int[,] MapTiles { get; set; }

        public MAPSettings(Stream stream) {
            CameraOrigin = stream.ReadIVector3();
            CameraTranslation = stream.ReadIVector3();

            Lights = new Light[3];
            for (int i = 0; i < 3; ++i) {
                var lightPosition = stream.ReadIVector3();
                var lightColor = stream.ReadIVector3();

                Lights[i] = new Light(lightPosition, lightColor);
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

        public void write(Stream stream) {
            stream.WriteIVector3(CameraOrigin);
            stream.WriteIVector3(CameraTranslation);
            for (int i = 0; i < 3; ++i) {
                Light light = Lights[i];
                stream.WriteIVector3(light.Position);
                stream.WriteIVector3(light.Color);
            }

            stream.Seek(12, SeekOrigin.Current);

            stream.WriteUShort(Zoom);
            stream.WriteUShort(SpriteScale);

            for (int i = 0; i < 4; ++i) {
                stream.WriteInt(AreaLikeTypes[i]);
            }

            for (int i = 0; i < 4; ++i) {
                stream.WriteInt(AreaDislikeTypes[i]);
            }

            stream.WriteInt(MapTileWidth);
            stream.WriteInt(MapTileHeight);
            for (int y = 0; y < MapTileHeight; ++y) {
                for (int x = 0; x < MapTileWidth; ++x) {
                    stream.WriteInt(MapTiles[x, y]);
                }
            }
        }
    }
}
