using MomenTFS.Extensions;
using MomenTFS.MAP.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.Sections
{
    public class MAPSettings
    {
        public Vector3 CameraOrigin { get; private set; }
        public Vector3 CameraTranslation { get; private set; }
        public List<Light> Lights { get; private set; }
        public ushort Zoom { get; private set; }
        public ushort SpriteScale { get; private set; }
        public ushort[] AreaLikeTypes { get; private set; }
        public ushort[] AreaDislikeTypes { get; private set; }
        public ushort MapTileWidth { get; private set; }
        public ushort MapTileHeight { get; private set; }
        public int[,] MapTiles { get; private set; }

        public MAPSettings(Stream stream) {
            CameraOrigin = stream.ReadVector3();
            CameraTranslation = stream.ReadVector3();

            Lights = new List<Light>();
            for (int i = 0; i < 3; ++i) {
                var lightPosition = stream.ReadVector3();
                var lightColor = stream.ReadVector3();

                Lights.Add(new Light(lightPosition, lightColor));
            }

            stream.Seek(24, SeekOrigin.Current);

            Zoom = stream.ReadUShort();
            SpriteScale = stream.ReadUShort();

            AreaLikeTypes = new ushort[4];
            AreaLikeTypes[0] = stream.ReadUShort();
            AreaLikeTypes[1] = stream.ReadUShort();
            AreaLikeTypes[2] = stream.ReadUShort();
            AreaLikeTypes[3] = stream.ReadUShort();

            AreaDislikeTypes = new ushort[4];
            AreaDislikeTypes[0] = stream.ReadUShort();
            AreaDislikeTypes[1] = stream.ReadUShort();
            AreaDislikeTypes[2] = stream.ReadUShort();
            AreaDislikeTypes[3] = stream.ReadUShort();

            MapTileWidth = stream.ReadUShort();
            MapTileHeight = stream.ReadUShort();
        }
    }
}
