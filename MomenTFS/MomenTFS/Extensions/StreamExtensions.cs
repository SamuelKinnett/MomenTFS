using MomenTFS.MAP.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.Extensions
{
    public static class StreamExtensions
    {
        public static ushort ReadUShort(this Stream stream) {
            return BitConverter.ToUInt16(stream.ReadBytes(2), 0);
        }

        public static short ReadShort(this Stream stream) {
            return BitConverter.ToInt16(stream.ReadBytes(2), 0);
        }

        private static byte[] ReadBytes(this Stream stream, int byteCount) {
            byte[] bytes = new byte[byteCount];

            for (int i = 0; i < byteCount; ++i) {
                bytes[i] = (byte) stream.ReadByte();
            }

            return bytes;
        }

        public static int ReadInt(this Stream stream) {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)stream.ReadByte();
            bytes[1] = (byte)stream.ReadByte();
            bytes[2] = (byte)stream.ReadByte();
            bytes[3] = (byte)stream.ReadByte();

            return BitConverter.ToInt32(bytes, 0);
        }

        public static Vector3 ReadVector3(this Stream stream) {
            return new Vector3(stream.ReadShort(), stream.ReadShort(), stream.ReadShort());
        }
    }
}
