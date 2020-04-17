using MomenTFS.MAP.Objects;
using MomenTFS.MAP.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP
{
    public class MAPReader
    {
        public MAPData Read(string filename) {
            using (FileStream stream = new FileStream(filename, FileMode.Open)) {
                return Read(stream);
            }
        }

        public MAPData Read(Stream stream) {
            MAPData mapData = new MAPData();

            byte firstAddress = (byte)stream.ReadByte();
            int sectionCount = firstAddress / 4;

            stream.Seek(firstAddress, SeekOrigin.Begin);

            mapData.Settings = new MAPSettings(stream);

            List<TIMImage> timImages = new List<TIMImage>();
            for (int i = 1; i < sectionCount - 3; ++i) {
                timImages.Add(new TIMImage(stream));
            }

            return mapData;
        }
    }
}
