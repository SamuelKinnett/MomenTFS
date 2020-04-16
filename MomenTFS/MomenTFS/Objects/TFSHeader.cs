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

        public TFSHeader(Stream stream) {
            Width = (byte)stream.ReadByte();
            RW = (byte)stream.ReadByte();
            Height = (byte)stream.ReadByte();
            RH = (byte)stream.ReadByte();
            PaletteCount = (byte)stream.ReadByte();
            RP = (byte)stream.ReadByte();
            Resl = stream.ReadUShort();
        }
    }
}
