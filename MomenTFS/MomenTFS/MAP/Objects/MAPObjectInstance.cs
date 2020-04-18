using MomenTFS.Extensions;
using System.IO;

namespace MomenTFS.MAP.Objects
{
    public class MAPObjectInstance
    {
        public ushort[] AnimationState { get; private set; }
        public ushort[] AnimationDuration { get; private set; }
        public ushort X { get; private set; }
        public ushort Y { get; private set; }

        public MAPObjectInstance(Stream stream) {
            AnimationState = new ushort[8];
            AnimationDuration = new ushort[8];
            
            for (int i = 0; i < 8; ++i) {
                AnimationState[i] = stream.ReadUShort();
            }

            for (int i = 0; i < 8; ++i) {
                AnimationDuration[i] = stream.ReadUShort();
            }

            X = stream.ReadUShort();
            Y = stream.ReadUShort();

            stream.ReadUShort();
        }
    }
}
