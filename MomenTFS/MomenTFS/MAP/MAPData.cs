using MomenTFS.MAP.Elements;
using MomenTFS.MAP.Objects;
using MomenTFS.MAP.Settings;
using MomenTFS.MAP.TIM;
using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP
{
    public class MAPData
    {
        public MAPSettings Settings { get; set; }
        public List<TIMImage> TIMImages { get; set; }
        public MAPObjects Objects { get; set; }
        public MAPElements Elements { get; set; }
    }
}
