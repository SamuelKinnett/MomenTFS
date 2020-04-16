using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.Objects
{
    public class Light
    {
        public Vector3 Position;
        public Vector3 Color;

        public Light(Vector3 position, Vector3 color) {
            Position = position;
            Color = color;
        }
    }
}
