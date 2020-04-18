using MomenTFS.Extensions;
using System.IO;

namespace MomenTFS.MAP.Objects
{
    public class MAPObjects
    {
        public ushort ObjectCount { get; private set; }
        public MAPObject[] Objects { get; private set; }
        public ushort InstanceCount { get; private set; }
        public MAPObjectInstance[] Instances { get; private set; }

        public MAPObjects(Stream stream) {
            ObjectCount = stream.ReadUShort();
            Objects = new MAPObject[ObjectCount];

            for (int i = 0; i < ObjectCount; ++i) {
                Objects[i] = new MAPObject(stream);
            }

            InstanceCount = stream.ReadUShort();
            Instances = new MAPObjectInstance[InstanceCount];

            for (int i = 0; i < InstanceCount; ++i) {
                Instances[i] = new MAPObjectInstance(stream);
            }
        }
    }
}
