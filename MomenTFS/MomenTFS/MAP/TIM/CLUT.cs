using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.TIM
{
    public class CLUT
    {
        public int Length { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public CLUTColor[,] LookupTable { get; set; }
    }
}
