using MomenTFS.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.Objects
{
    public class TFSHeader
    {
        public byte Width { get; set; }
        public byte RW { get; set; }
        public byte Height { get; set; }
        public byte RH { get; set; }
        public byte PaletteCount { get; set; }
        public byte RP { get; set; }
        public ushort Resl { get; set; }

        public TFSHeader(FileStream fileStream) {
            Width = (byte)fileStream.ReadByte();
            RW = (byte)fileStream.ReadByte();
            Height = (byte)fileStream.ReadByte();
            RH = (byte)fileStream.ReadByte();
            PaletteCount = (byte)fileStream.ReadByte();
            RP = (byte)fileStream.ReadByte();
            Resl = fileStream.ReadShort();
        }
    }
}
