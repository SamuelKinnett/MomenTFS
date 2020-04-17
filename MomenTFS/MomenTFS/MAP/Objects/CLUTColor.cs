using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.Objects
{
    public class CLUTColor
    {
        public byte Red { get; private set; }
        public byte Green { get; private set; }
        public byte Blue { get; private set; }
        public bool SpecialTransparencyProcessing { get; private set; }

        public CLUTColor(byte red, byte green, byte blue, bool stp) {
            Red = red;
            Green = green;
            Blue = blue;
            SpecialTransparencyProcessing = stp;
        }

        public CLUTColor(ushort data) {
            Red = (byte)(data & 0x1F);
            Green = (byte)((data >> 5) & 0x1F);
            Blue = (byte)((data >> 10) & 0x1F);
            SpecialTransparencyProcessing = (data >> 15 & 0x01) > 0;
        }

        public int[] GetAsRGBA() {
            var rgba = new int[4];

            // If the special transparency bit is set then this colour is treated as transparent,
            // UNLESS it is black which by default is treated as transparent and only opaque if the
            // STP bit is set.

            rgba[0] = Red * 8;
            rgba[1] = Green * 8;
            rgba[2] = Blue * 8;
            rgba[3] = Red == 0 && Green == 0 && Blue == 0
                ? SpecialTransparencyProcessing
                    ? 0
                    : 255
                : SpecialTransparencyProcessing
                    ? 255
                    : 0;

            return rgba;
        }
    }
}
