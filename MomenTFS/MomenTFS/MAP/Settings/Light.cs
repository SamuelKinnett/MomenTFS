using MomenTFS.Objects;

namespace MomenTFS.MAP.Settings
{
    public class Light
    {
        public IVector3 Position;
        public IVector3 Color;

        public Light(IVector3 position, IVector3 color) {
            Position = position;
            Color = color;
        }

        public override string ToString() {
            return $"Position: {Position}, Color: {Color}";
        }
    }
}
