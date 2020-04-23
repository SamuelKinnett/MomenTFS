namespace MomenTFS.Objects
{
    public class IVector3 : IVector2
    {
        public int Z { get; set; }

        public IVector3(int x, int y, int z) : base(x, y) {
            Z = z;
        }

        public override string ToString() {
            return $"[{X}, {Y}, {Z}]";
        }
    }
}
