using System;
using Eto.Forms;
using MomenTFS.Forms;

namespace MomenTFS.Wpf
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args) {
            new Application(Eto.Platforms.Wpf).Run(new MainForm());
        }
    }
}
