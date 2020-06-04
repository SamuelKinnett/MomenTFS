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
        public short Unknown1 { get; private set; }
        public ushort Z { get; private set; }
        public short Unknown2 { get; private set; }
        public short Unknown3 { get; private set; }
        public short Unknown4 { get; private set; }

        public MAPObject(Stream stream) {
            SpritesheetX = stream.ReadUShort();
            SpritesheetY = stream.ReadUShort();
            Width = stream.ReadUShort();
            Height = stream.ReadUShort();

            Unknown1 = stream.ReadShort();

            Z = stream.ReadUShort();

            Unknown2 = stream.ReadShort();
            Unknown3 = stream.ReadShort();
            Unknown4 = stream.ReadShort();
        }
        //199150
        public override string ToString() {
            return $"SpriteX: {SpritesheetX}\nSpriteY: {SpritesheetY}\nSprite Width: {Width}\n" +
                $"Sprite Height: {Height}\nZ: {Z}\nUnknown 1: {Unknown1}\n" +
                $"Unknown 2: {Unknown2}\nUnknown 3: {Unknown3}\nUnknown 4: {Unknown4}";
        }
    }
}
