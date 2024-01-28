using MomenTFS.MAP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS {
    public class RoomWriter {

        private MAPWriter mapWriter;

        public RoomWriter() {
            mapWriter = new MAPWriter();
        }

        public void WriteRoomDataMAPOnly(String mapFile, RoomData data) {
            using (FileStream fileStream = new FileStream(mapFile, FileMode.Open)) {
                WriteRoomDataMAPOnly(fileStream, data);
            }
        }

        public void WriteRoomDataMAPOnly(Stream stream, RoomData data) {
            mapWriter.Patch(stream, data.MAPData);
        }
    }
}
