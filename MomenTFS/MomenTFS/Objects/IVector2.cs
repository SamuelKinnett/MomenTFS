namespace MomenTFS.Objects
{
    public class IVector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IVector2(int x, int y) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return $"[{X}, {Y}]";
        }
    }
}
