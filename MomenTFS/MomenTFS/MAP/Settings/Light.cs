using MomenTFS.Objects;

namespace MomenTFS.MAP.Settings
{
    public class Light
    {
        public IntVector3 Position;
        public IntVector3 Color;

        public Light(IntVector3 position, IntVector3 color) {
            Position = position;
            Color = color;
        }
    }
}
