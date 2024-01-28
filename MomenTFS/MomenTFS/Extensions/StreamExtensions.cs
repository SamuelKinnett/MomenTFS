using MomenTFS.MAP.Enums;
using MomenTFS.Objects;
using System;
using System.IO;

namespace MomenTFS.Extensions {
    public static class StreamExtensions {
        public static ushort ReadUShort(this Stream stream, Endian endianness = Endian.BIG) {
            return BitConverter.ToUInt16(stream.ReadBytes(2, endianness), 0);
        }

        public static void WriteUShort(
                this Stream stream, ushort value, Endian endianness = Endian.BIG) {
            stream.WriteBytes(BitConverter.GetBytes(value), endianness);
        }

        public static short ReadShort(this Stream stream, Endian endianness = Endian.BIG) {
            return BitConverter.ToInt16(stream.ReadBytes(2, endianness), 0);
        }

        public static void WriteShort(
                this Stream stream, short value, Endian endianness = Endian.BIG) {
            stream.WriteBytes(BitConverter.GetBytes(value), endianness);
        }

        public static int ReadInt(this Stream stream, Endian endianness = Endian.BIG) {
            byte[] bytes = stream.ReadBytes(4, endianness);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static void WriteInt(
                this Stream stream, int value, Endian endianness = Endian.BIG) {
            stream.WriteBytes(BitConverter.GetBytes(value), endianness);
        }

        public static SVector3 ReadSVector3(
                this Stream stream, Endian endianness = Endian.BIG) {
            return new SVector3(
                stream.ReadShort(endianness),
                stream.ReadShort(endianness),
                stream.ReadShort(endianness));
        }

        public static void WriteSVector3(
                this Stream stream, SVector3 value, Endian endianness = Endian.BIG) {
            stream.WriteShort(value.X);
            stream.WriteShort(value.Y);
            stream.WriteShort(value.Z);
        }

        public static IVector3 ReadIVector3(
                this Stream stream, Endian endianness = Endian.BIG) {
            return new IVector3(
                stream.ReadInt(endianness),
                stream.ReadInt(endianness),
                stream.ReadInt(endianness));
        }

        public static void WriteIVector3(
                this Stream stream, IVector3 value, Endian endianness = Endian.BIG) {
            stream.WriteInt(value.X);
            stream.WriteInt(value.Y);
            stream.WriteInt(value.Z);
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

        private static void WriteBytes(this Stream stream, byte[] bytes, Endian endianness) {
            switch (endianness) {
                case Endian.BIG:
                    for (int i = 0; i < bytes.Length; ++i) {
                        stream.WriteByte(bytes[i]);
                    }
                    break;
                case Endian.LITTLE:
                    for (int i = bytes.Length - 1; i >= 0; --i) {
                        stream.WriteByte(bytes[i]);
                    }
                    break;
            }
        }
    }
}
