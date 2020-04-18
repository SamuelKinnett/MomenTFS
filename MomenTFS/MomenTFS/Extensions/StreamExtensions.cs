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

        public static ShortVector3 ReadShortVector3( 
                this Stream stream, Endian endianness = Endian.BIG) {
            return new ShortVector3(
                stream.ReadShort(endianness),
                stream.ReadShort(endianness),
                stream.ReadShort(endianness));
        }

        public static IntVector3 ReadIntVector3(
                this Stream stream, Endian endianness = Endian.BIG) {
            return new IntVector3(
                stream.ReadInt(endianness),
                stream.ReadInt(endianness),
                stream.ReadInt(endianness));
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
