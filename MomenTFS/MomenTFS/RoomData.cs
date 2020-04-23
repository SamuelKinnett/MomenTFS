using MomenTFS.MAP;
using MomenTFS.TFS;

namespace MomenTFS
{
    public class RoomData
    {
        public TFSData TFSData { get; private set; }
        public MAPData MAPData { get; private set; }

        public RoomData(TFSData tfsData, MAPData mapData) {
            TFSData = tfsData;
            MAPData = mapData;
        }
    }
}
