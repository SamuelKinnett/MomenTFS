using MomenTFS.MAP;
using MomenTFS.TFS;

namespace MomenTFS
{
    public class RoomData
    {
        public TFSData TFSData { get; set; }
        public MAPData MAPData { get; set; }

        public RoomData(TFSData tfsData, MAPData mapData) {
            TFSData = tfsData;
            MAPData = mapData;
        }
    }
}
