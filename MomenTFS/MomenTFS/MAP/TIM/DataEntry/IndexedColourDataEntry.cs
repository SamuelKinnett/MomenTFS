using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.TIM.DataEntry
{
    public class IndexedColourDataEntry : ImageDataEntry
    {
        public byte CLUTIndex { get; private set; }

        public IndexedColourDataEntry(byte clutIndex) {
            CLUTIndex = clutIndex;
        }
    }
}
