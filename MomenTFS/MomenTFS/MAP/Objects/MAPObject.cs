using MomenTFS.Extensions;
using System.IO;

namespace MomenTFS.MAP.Objects
{
    public class MAPObject
    {
        public ushort SpritesheetX { get; private set; }
        public ushort SpritesheetY { get; private set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
        public ushort Z { get; private set; }

        public MAPObject(Stream stream) {
            SpritesheetX = stream.ReadUShort();
            SpritesheetY = stream.ReadUShort();
            Width = stream.ReadUShort();
            Height = stream.ReadUShort();

            stream.ReadUShort();

            Z = stream.ReadUShort();

            for (int i = 0; i < 3; ++i) {
                stream.ReadUShort();
            }
        }
    }
}
