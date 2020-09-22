using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaCasinoHacker
{
    public class GtaFingerprintTarget : IGtaFingerprint
    {
        public GtaFingerprintTarget(bool[,] data) : base(data)
        {
        }

        public GtaFingerprintTarget(Bitmap image, float left, float top, float width, float height, float baseImageWidth, float baseImageHeight) : base(image, left, top, width, height, baseImageWidth, baseImageHeight)
        {
        }

        public override bool GetDataFromImagePixel(Color c, int x, int y)
        {
            //Get the highest value of all channels.
            //We do this to combat the cascading R-G-B tile GTA applies to the background
            byte color = Math.Max(c.R, Math.Max(c.G, c.B));

            //Start filtering. Check the background, then check the dots
            return !(color < 35) && !(color > 90);
        }
    }
}
