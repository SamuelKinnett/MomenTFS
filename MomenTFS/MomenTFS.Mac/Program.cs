using System;
using Eto.Forms;
using MomenTFS.Forms;

namespace MomenTFS.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args) {
            new Application(Eto.Platforms.Mac64).Run(new MainForm());
        }
    }
}
