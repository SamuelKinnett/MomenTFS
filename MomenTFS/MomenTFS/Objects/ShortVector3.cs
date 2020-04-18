namespace MomenTFS.Objects
{
    public class ShortVector3
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }

        public ShortVector3(short x, short y, short z) {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
