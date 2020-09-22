using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaCasinoHacker
{
    public abstract class IGtaFingerprint
    {
        public bool[,] data;

        public IGtaFingerprint(bool[,] data)
        {
            this.data = data;
        }

        public IGtaFingerprint(Bitmap image, float left, float top, float width, float height, float baseImageWidth, float baseImageHeight)
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

            //Make array
            data = new bool[finalWidth, finalHeight];

            //Copy
            for (int x = 0; x < newImage.Width; x++)
            {
                for (int y = 0; y < newImage.Height; y++)
                {
                    bool active = GetDataFromImagePixel(image.GetPixel(x + finalLeft, y + finalTop), x, y);
                    data[x, y] = active;
                }
            }
        }

        public int width { get { return data.GetLength(0); } }
        public int height { get { return data.GetLength(1); } }

        public abstract bool GetDataFromImagePixel(Color c, int x, int y);

        public bool[,] ResizeDataArray(int newWidth, int newHeight)
        {
            //Check if they are the same size. If not, we can skip this entirely
            if (newWidth == width && newHeight == height)
                return data;
            
            //Make array
            bool[,] newData = new bool[newWidth, newHeight];

            //Determine scaling
            float scaleX = (float)width / (float)newWidth;
            float scaleY = (float)height / (float)newHeight;

            //Copy
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newData[x, y] = data[(int)Math.Floor((float)x * scaleX), (int)Math.Floor((float)y * scaleY)];
                }
            }

            return newData;
        }

        public Bitmap ReadDataAsBitmap()
        {
            Bitmap bmp = new Bitmap(width, height);
            for(int x = 0; x<bmp.Width; x++)
            {
                for(int y = 0; y<bmp.Height; y++)
                {
                    //Make color
                    Color color;
                    if (data[x, y])
                        color = Color.White;
                    else
                        color = Color.Black;

                    //Set
                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }
    }
}
