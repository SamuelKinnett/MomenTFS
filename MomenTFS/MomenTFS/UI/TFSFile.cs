using Eto.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.UI
{
    public class TFSFile {
        public String Filename { get; set; }
        public String DiscFile { get; set; }

        public TFSFile(string filename, string discFile = null) {
            Filename = filename;
            DiscFile = discFile;
        }
    }
}
