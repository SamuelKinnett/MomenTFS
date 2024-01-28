using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.MAP {
    public class MAPWriter {
        public void Patch(string filename, MAPData data) {
            using (FileStream stream = new FileStream(filename, FileMode.Open)) {
                Patch(filename, data);
            }
        }

        public void Patch(Stream stream, MAPData data) {
            byte firstAddress = (byte)stream.ReadByte();
            stream.Seek(firstAddress, SeekOrigin.Begin);

            data.Settings.write(stream);
        }
    }
}
