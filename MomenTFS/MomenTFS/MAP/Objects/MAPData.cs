using MomenTFS.MAP.Sections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MomenTFS.MAP.Objects
{
    public class MAPData
    {
        public MAPSettings Settings { get; set; }
        public List<TIMImage> TIMImages { get; set; }
    }
}
