using MomenTFS.Extensions;
using System.IO;

namespace MomenTFS.MAP.Objects
{
    public class MAPObjectInstance
    {
        public short[] AnimationState { get; private set; }
        public short[] AnimationDuration { get; private set; }
        public ushort X { get; private set; }
        public ushort Y { get; private set; }

        public MAPObjectInstance(Stream stream) {
            AnimationState = new short[8];
            AnimationDuration = new short[8];
            
            for (int i = 0; i < 8; ++i) {
                AnimationState[i] = stream.ReadShort();
            }

            for (int i = 0; i < 8; ++i) {
                AnimationDuration[i] = stream.ReadShort();
            }

            X = stream.ReadUShort();
            Y = stream.ReadUShort();

            stream.Seek(2, SeekOrigin.Current);
        }
    }
}
