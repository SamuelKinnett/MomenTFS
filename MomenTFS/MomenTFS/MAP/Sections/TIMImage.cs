using MomenTFS.Extensions;
using MomenTFS.MAP.Enums;
using MomenTFS.MAP.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.Sections
{
    public class TIMImage
    {
        public byte Version { get; set; }
        public BitsPerPixel BitsPerPixel { get; set; }
        public bool CLUTPresent { get; set; }
        public CLUT ColourLookupTable { get; set; }
        public int ImageLength { get; private set; }
        public ushort ImageX { get; set; }
        public ushort ImageY { get; set; }
        public ushort ImageWidth { get; set; }
        public ushort ImageHeight { get; set; }

        public TIMImage(Stream stream) {
            byte tag = (byte)stream.ReadByte();

            if (tag != 16) {
                throw new InvalidOperationException(
                    "The first byte found in the stream was not 0x10; the data is not in the TIM" +
                    " format");
            }

            Version = (byte)stream.ReadByte();

            stream.Seek(2, SeekOrigin.Current);

            byte flags = (byte)stream.ReadByte();
            BitsPerPixel = (BitsPerPixel)(flags & 0x03);
            CLUTPresent = (flags & 0x08) > 0;

            stream.Seek(3, SeekOrigin.Current);

            if (CLUTPresent) {
                CLUT clut = new CLUT();
                clut.Length = stream.ReadInt();
                clut.X = stream.ReadUShort();
                clut.Y = stream.ReadUShort();
                clut.Width = stream.ReadUShort();
                clut.Height = stream.ReadUShort();

                CLUTColor[,] palette = new CLUTColor[clut.Width, clut.Height];

                for (int y = 0; y < clut.Height; ++y) {
                    for (int x = 0; x < clut.Width; ++x) {
                        palette[x, y] = new CLUTColor(stream.ReadUShort());
                    }
                }
            }



            var test = 2;
        }
    }
}
