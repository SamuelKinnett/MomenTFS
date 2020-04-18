using MomenTFS.Extensions;
using MomenTFS.MAP.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP.TIM.DataEntry
{
    public class ImageDataEntryFactory
    {
        /// <summary>
        /// Creates image data entries from the provided stream for the given BPP
        /// </summary>
        public static ImageDataEntry[] CreateImageDataEntry(
                List<ushort> frameBufferPixels, BitsPerPixel bitsPerPixel) {
            switch (bitsPerPixel) {
                case BitsPerPixel.FOUR: {
                        ImageDataEntry[] imageDataEntries = new ImageDataEntry[4];
                        ushort data = frameBufferPixels[0];

                        imageDataEntries[0]
                            = new IndexedColourDataEntry((byte)(data & 0x0F));
                        imageDataEntries[1]
                            = new IndexedColourDataEntry((byte)((data >> 4) & 0x0F));
                        imageDataEntries[2]
                            = new IndexedColourDataEntry((byte)((data >> 8) & 0x0F));
                        imageDataEntries[3]
                            = new IndexedColourDataEntry((byte)((data >> 12) & 0x0F));

                        return imageDataEntries;
                    }
                case BitsPerPixel.EIGHT: {
                        ImageDataEntry[] imageDataEntries = new ImageDataEntry[2];
                        ushort data = frameBufferPixels[0];

                        imageDataEntries[0]
                            = new IndexedColourDataEntry((byte)(data & 0xFF));
                        imageDataEntries[1]
                            = new IndexedColourDataEntry((byte)((data >> 8) & 0xFF));

                        return imageDataEntries;
                    }
                case BitsPerPixel.SIXTEEN: {
                        ImageDataEntry[] imageDataEntries = new ImageDataEntry[1];

                        imageDataEntries[0] = new RealColourDataEntry(frameBufferPixels[0]);

                        return imageDataEntries;
                    }
                case BitsPerPixel.TWENTY_FOUR: {
                        ImageDataEntry[] imageDataEntries = new ImageDataEntry[1];

                        imageDataEntries[0]
                            = new RealColourDataEntry(
                                frameBufferPixels[0],
                                frameBufferPixels[1],
                                frameBufferPixels[2]);

                        return imageDataEntries;
                    }
                default:
                    throw new InvalidOperationException(
                        $"No factory method found for BitsPerPixel value " +
                        $"'{bitsPerPixel}'");
            }
        }
    }
}
