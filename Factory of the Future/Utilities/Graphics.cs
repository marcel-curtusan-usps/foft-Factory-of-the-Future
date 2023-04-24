using System;
using System.Drawing;
using System.IO;

namespace Factory_of_the_Future
{
    public static class Graphics
    {
        public static string HighlightBase64Image(string base64, int r, int g, int b, double a)
        {
            Image img = Base64StringToImage(base64);
            Bitmap bmp = Highlight(img, r, g, b, a);
            return BitmapToBase64String(bmp);
        }
        public static Bitmap Highlight(Image img, int r, int g, int b, double a)
        {
            int x = 0;
            int y = 0;
            int newR = 0;
            int newG = 0;
            int newB = 0;
            Color pixelColor = new Color();
            Bitmap newBmp = new Bitmap(img);
            for (x = 0; x < newBmp.Width; x++)
            {
                for (y = 0; y < newBmp.Height; y++)
                {
                    pixelColor = newBmp.GetPixel(x, y);
                    newR = ((int)pixelColor.R + (int)(r * a)) / 2;
                    newG = ((int)pixelColor.G + (int)(g * a)) / 2;
                    newB = ((int)pixelColor.B + (int)(b * a)) / 2;
                    pixelColor = Color.FromArgb(newR, newG, newB);
                    newBmp.SetPixel(x, y, pixelColor);
                }
            }
            return newBmp;
        }
        public static Image Base64StringToImage(string
                                            base64String)
        {
            Image bmpReturn = null;


            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);


            memoryStream.Position = 0;


            bmpReturn = (Image)Bitmap.FromStream(memoryStream);


            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;


            return bmpReturn;
        }
        public static string BitmapToBase64String(Bitmap
                                           bmp)
        {

            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bmpBytes = stream.ToArray();
                string base64Image = Convert.ToBase64String(bmpBytes);
                return base64Image;
            }


        }
    }
}