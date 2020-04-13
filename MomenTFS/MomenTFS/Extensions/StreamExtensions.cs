using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.Extensions
{
    public static class StreamExtensions
    {
        public static ushort ReadShort(this Stream stream) {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)stream.ReadByte();
            bytes[1] = (byte)stream.ReadByte();

            return BitConverter.ToUInt16(bytes, 0);
        }
    }
}
