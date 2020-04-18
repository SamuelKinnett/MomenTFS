using System;

namespace MomenTFS.UI
{
    public class TFSFile {
        public string Filename { get; set; }
        public string MAPFilename { get; set; }
        public string DiscFile { get; set; }

        public TFSFile(string filename, string discFile = null) {
            Filename = filename;
            DiscFile = discFile;
        }
    }
}
