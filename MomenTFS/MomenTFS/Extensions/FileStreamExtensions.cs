using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.Extensions
{
    public static class FileStreamExtensions
    {
        public static ushort ReadShort(this FileStream fileStream) {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)fileStream.ReadByte();
            bytes[1] = (byte)fileStream.ReadByte();

            return BitConverter.ToUInt16(bytes, 0);
        }
    }
}
