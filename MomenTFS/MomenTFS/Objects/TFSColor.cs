namespace MomenTFS.Objects
{
    public class TFSColor {
        public byte Red {get;set;}
        public byte Blue { get; set; }
        public byte Green { get; set; }

        public TFSColor(byte red, byte green, byte blue) {
            Red = red;
            Blue = blue;
            Green = green;
        }
    }
}
