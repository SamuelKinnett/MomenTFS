using MomenTFS.MAP;
using MomenTFS.TFS;
using System.IO;

namespace MomenTFS
{
    public class RoomReader
    {
        private TFSReader tfsReader;
        private MAPReader mapReader;

        public RoomReader() {
            tfsReader = new TFSReader();
            mapReader = new MAPReader();
        }

        public RoomData ReadRoomDataTFSOnly(string tfsFile) {
            using (FileStream fileStream = new FileStream(tfsFile, FileMode.Open)) {
                return ReadRoomDataTFSOnly(fileStream);
            }
        }

        public RoomData ReadRoomDataTFSOnly(Stream tfsStream) {
            return new RoomData(tfsReader.Read(tfsStream), null);
        }

        public RoomData ReadRoomData(string tfsFile, string mapFile) {
            using (FileStream tfsStream = new FileStream(tfsFile, FileMode.Open))
            using (FileStream mapStream = new FileStream(mapFile, FileMode.Open)) {
                return ReadRoomData(tfsStream, mapStream);
            }
        }

        public RoomData ReadRoomData(Stream tfsStream, Stream mapStream) {
            return new RoomData(tfsReader.Read(tfsStream), mapReader.Read(mapStream));
        }
    }
}
