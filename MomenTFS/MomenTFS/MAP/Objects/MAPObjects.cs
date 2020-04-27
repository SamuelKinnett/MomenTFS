using MomenTFS.Extensions;
using System;
using System.IO;

namespace MomenTFS.MAP.Objects
{
    public class MAPObjects
    {
        public short ObjectCount { get; private set; }
        public MAPObject[] Objects { get; private set; }
        public short InstanceCount { get; private set; }
        public MAPObjectInstance[] Instances { get; private set; }

        public MAPObjects(Stream stream) {
            ObjectCount = Math.Max(stream.ReadShort(), (short)0);
            Objects = new MAPObject[ObjectCount];

            for (int i = 0; i < ObjectCount; ++i) {
                Objects[i] = new MAPObject(stream);
            }

            InstanceCount = Math.Max(stream.ReadShort(), (short)0);
            Instances = new MAPObjectInstance[InstanceCount];

            for (int i = 0; i < InstanceCount; ++i) {
                Instances[i] = new MAPObjectInstance(stream);
            }
        }
    }
}
