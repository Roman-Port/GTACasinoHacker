using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaCasinoHacker
{
    public class GtaScreenshot
    {
        public Bitmap image;
        public bool is169;
        public bool isWindowed;
        public bool imageResized;

        public GtaScreenshot(Bitmap image, bool isWindowed)
        {
            //Check if we need to resize
            imageResized = isWindowed || image.Width != 1920 || image.Height != 1080;
            if (imageResized)
            {
                //If it is windowed, we need to trim off the window border
                int paddingTop = isWindowed ? 26 : 0;
                int paddingLeft = isWindowed ? 3 : 0;
                int paddingRight = isWindowed ? 3 : 0;
                int paddingBottom = isWindowed ? 4 : 0;

                //Check if this is 16:9
                float aspect = (float)(image.Width - paddingLeft - paddingRight) / (float)(image.Height - paddingTop - paddingBottom);
                is169 = aspect <= 1.8f && aspect >= 1.7f;

                //Resize this to 1080p. This is a hack
                Bitmap result = new Bitmap(1920, 1080);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(image, new Rectangle(0, 0, 1920, 1080), paddingLeft, paddingTop, image.Width - paddingLeft - paddingRight, image.Height - paddingTop - paddingBottom, GraphicsUnit.Pixel);
                }
                this.image = result;
            } else
            {
                //Current image is good
                is169 = true;
                this.image = image;
            }
        }

        public int width { get { return image.Width; } }
        public int height { get { return image.Height; } }

        /// <summary>
        /// Returns the large fingerprint as a bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap GetCloneTargetBitmap()
        {
            //Get the fingerprint cropped
            Bitmap fingerprint = ScaledImageCrop(911, 153, 470, 530, 1920, 1080);

            return fingerprint;
        }

        /// <summary>
        /// Returns one of the 8 targets on the left as a bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetComponentBitmap(int leftIndex, int topIndex)
        {
            //Get the fingerprint cropped
            Bitmap fingerprint = ScaledImageCrop(480 + (145 * leftIndex), 277 + (144 * topIndex), 106, 106, 1920, 1080);

            return fingerprint;
        }

        public GtaFingerprintTarget GetCloneTarget()
        {
            //return new GtaFingerprintTarget(GetCloneTargetBitmap());
            return new GtaFingerprintTarget(image, 911, 153, 470, 530, 1920, 1080);
        }

        public GtaFingerprintChallenge GetComponent(int leftIndex, int topIndex)
        {
            //var f = new GtaFingerprintChallenge(GetComponentBitmap(leftIndex, topIndex));
            var f = new GtaFingerprintChallenge(image, 480 + (145 * leftIndex), 277 + (144 * topIndex), 106, 106, 1920, 1080);
            f.fingerX = leftIndex;
            f.fingerY = topIndex;
            return f;
        }

        /// <summary>
        /// Crops the base image and scales it to the resolution of the screen
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="baseImageWidth"></param>
        /// <param name="baseImageHeight"></param>
        /// <returns></returns>
        private Bitmap ScaledImageCrop(float left, float top, float width, float height, float baseImageWidth, float baseImageHeight)
        {
            //Deteremine scale factor
            float scaleFactor = (float)image.Width / baseImageWidth;

            //Calculate
            int finalLeft = (int)(left * scaleFactor);
            int finalTop = (int)(top * scaleFactor);
            int finalWidth = (int)(width * scaleFactor);
            int finalHeight = (int)(height * scaleFactor);

            //Create image
            Bitmap newImage = new Bitmap(finalWidth, finalHeight);

            //Copy
            for(int x = 0; x<newImage.Width; x++)
            {
                for(int y = 0; y<newImage.Height; y++)
                {
                    newImage.SetPixel(x, y, image.GetPixel(x + finalLeft, y + finalTop));
                }
            }

            return newImage;
        }
    }
}
