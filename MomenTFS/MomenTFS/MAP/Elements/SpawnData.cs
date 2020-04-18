namespace MomenTFS.MAP.Elements
{
    public class SpawnData
    {
        public short X { get; private set; }
        public short Y { get; private set; }
        public short Z { get; private set; }
        public short Rotation { get; private set; }

        public SpawnData(short x, short y, short z, short rotation) {
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
        }
    }
}
