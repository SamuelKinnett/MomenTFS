using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MomenTFS.Forms.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap ToEtoBitmap(this System.Drawing.Bitmap systemBitmap) {
            Bitmap etoBitmap;

            using (MemoryStream memoryStream = new MemoryStream()) {
                systemBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                etoBitmap = new Bitmap(memoryStream);
            }

            return etoBitmap;
        }
    }
}
