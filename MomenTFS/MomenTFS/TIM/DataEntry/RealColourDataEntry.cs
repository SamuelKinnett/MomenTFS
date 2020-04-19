using System.Drawing;

namespace MomenTFS.TIM.DataEntry
{
    public class RealColourDataEntry : ImageDataEntry
    {
        public Color Color { get; set; }

        public RealColourDataEntry(ushort data) {
            CLUTColor color = new CLUTColor(data);
            var rgba = color.GetAsRGBA();

            Color = Color.FromArgb(rgba[3], rgba[0], rgba[1], rgba[2]);
        }

        public RealColourDataEntry(ushort red, ushort green, ushort blue) {
            Color = Color.FromArgb(red, green, blue);
        }
    }
}
