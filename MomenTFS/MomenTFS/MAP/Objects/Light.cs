using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.Objects
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
