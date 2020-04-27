using MomenTFS.MAP.Enums;
using MomenTFS.Objects;
using System;
using System.IO;

namespace MomenTFS.Extensions
{
    public static class StreamExtensions
    {
        public static ushort ReadUShort(this Stream stream, Endian endianness = Endian.BIG) {
            return BitConverter.ToUInt16(stream.ReadBytes(2, endianness), 0);
        }

        public static short ReadShort(this Stream stream, Endian endianness = Endian.BIG) {
            return BitConverter.ToInt16(stream.ReadBytes(2, endianness), 0);
        }

        public static int ReadInt(this Stream stream, Endian endianness = Endian.BIG) {
            byte[] bytes = stream.ReadBytes(4, endianness);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static SVector3 ReadSVector3( 
                this Stream stream, Endian endianness = Endian.BIG) {
            short x = stream.ReadShort(endianness);
            short z = stream.ReadShort(endianness);
            short y = stream.ReadShort(endianness);

            return new SVector3(x, y, z);
        }

        public static IVector3 ReadIVector3(
                this Stream stream, Endian endianness = Endian.BIG) {
            int x = stream.ReadInt(endianness);
            int z = stream.ReadInt(endianness);
            int y = stream.ReadInt(endianness);

            return new IVector3(x, y, z);
        }

        private static byte[] ReadBytes(this Stream stream, int byteCount, Endian endianness) {
            byte[] bytes = new byte[byteCount];

            switch (endianness) {
                case Endian.BIG:
                    for (int i = 0; i < byteCount; ++i) {
                        bytes[i] = (byte)stream.ReadByte();
                    }
                    break;
                case Endian.LITTLE:
                    for (int i = byteCount - 1; i >= 0; --i) {
                        bytes[i] = (byte)stream.ReadByte();
                    }
                    break;
            }

            return bytes;
        }
    }
}
