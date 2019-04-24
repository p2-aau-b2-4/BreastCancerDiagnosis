using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePreprocessing
{
    public class UByteArrayAsImage : ArrayAsImageAbstract
    {
        private byte[,] _pixelArray;

        public byte[,] PixelArray
        {
            get
            {
                if (_pixelArray == null)
                {
                    // lets generate it from PixelData
                    byte[,] toReturn = new byte[Width, Height];
                    for (var i = 0; i < PixelData.Length; i++)
                    {
                        toReturn[i % Width, i / Width] = PixelData[i];
                    }

                    _pixelArray = toReturn;
                }

                return _pixelArray;
            }
        }
        
        public UByteArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }

        public override Stream GetPngAsMemoryStream()
        {
            Bitmap imgBitmap = new Bitmap(PixelArray.GetLength(0), PixelArray.GetLength(1));
            for (int x = 0; x < PixelArray.GetLength(0); x++)
            {
                for (int y = 0; y < PixelArray.GetLength(1); y++)
                {
                    int greyColor = PixelArray[x, y];
                    imgBitmap.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                }
            }

            // lets add all bitmaps:
            Graphics g = Graphics.FromImage(imgBitmap);
            g.CompositingMode = CompositingMode.SourceOver;
            foreach (Bitmap bitmap in Overlays)
            {
                bitmap.MakeTransparent();
                g.DrawImage(bitmap, new Point(0, 0));
            }

            MemoryStream ms = new MemoryStream();
            imgBitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}