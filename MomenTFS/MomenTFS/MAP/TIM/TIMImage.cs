using MomenTFS.Extensions;
using MomenTFS.MAP.Enums;
using MomenTFS.MAP.TIM.DataEntry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.TIM
{
    public class TIMImage
    {
        public byte Version { get; private set; }
        public BitsPerPixel BitsPerPixel { get; private set; }
        public bool HasCLUT { get; private set; }
        public CLUT ColourLookupTable { get; private set; }
        public int ImageLength { get; private set; }
        public ushort ImageX { get; private set; }
        public ushort ImageY { get; private set; }
        public ushort ImageWidth { get; private set; }
        public ushort ImageHeight { get; private set; }
        public ImageDataEntry[,] ImageData { get; private set; }

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
            HasCLUT = (flags & 0x08) > 0;

            stream.Seek(3, SeekOrigin.Current);

            if (HasCLUT) {
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

            ImageLength = stream.ReadInt();
            ImageX = stream.ReadUShort();
            ImageY = stream.ReadUShort();
            ImageWidth = stream.ReadUShort();
            ImageHeight = stream.ReadUShort();

            int wordsPerDataEntry = 1;

            switch (BitsPerPixel) {
                case BitsPerPixel.FOUR:
                    ImageData = new ImageDataEntry[ImageWidth * 4, ImageHeight];
                    break;
                case BitsPerPixel.EIGHT:
                    ImageData = new ImageDataEntry[ImageWidth * 2, ImageHeight];
                    break;
                case BitsPerPixel.SIXTEEN:
                    ImageData = new ImageDataEntry[ImageWidth, ImageHeight];
                    break;
                case BitsPerPixel.TWENTY_FOUR:
                    ImageData
                        = new ImageDataEntry[(int)Math.Ceiling(ImageWidth / 3f), ImageHeight];
                    wordsPerDataEntry = 3;
                    break;
            }

            for (int y = 0; y < ImageHeight; ++y) {
                List<ushort> buffer = new List<ushort>();
                List<ImageDataEntry> currentRow = new List<ImageDataEntry>();

                for (int x = 0; x < ImageWidth; ++x) {
                    buffer.Add(stream.ReadUShort());

                    if (buffer.Count == wordsPerDataEntry) {
                        currentRow.AddRange(
                            ImageDataEntryFactory.CreateImageDataEntry(buffer, BitsPerPixel));
                        buffer.Clear();
                    }
                }

                for (int x = 0; x < currentRow.Count; ++x) {
                    ImageData[x, y] = currentRow[x];
                }
            }
        }
    }
}
