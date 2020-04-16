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
            stream.Seek(firstAddress, SeekOrigin.Begin);

            mapData.Settings = new MAPSettings(stream);

            return mapData;
        }
    }
}
