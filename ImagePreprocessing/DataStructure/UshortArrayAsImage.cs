using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePreprocessing
{
    public class UshortArrayAsImage : ArrayAsImageAbstract
    {

        public ushort[,] PixelArray
        {
            get
            {
                ushort[,] result = new ushort[this.Height, this.Width];
                // this uses blockcopy, since data format is the same in byte[] and ushort[,]
                Buffer.BlockCopy(PixelData, 0, result, 0, PixelData.Length);
                return result;
            }
            set
            {
                byte[] result = new byte[this.Width * this.Height * 2];
                Buffer.BlockCopy(value, 0, result, 0, PixelData.Length);
                PixelData = result;
            }
        }

        public UshortArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }

        public override Stream GetPngAsMemoryStream()
        {
            var pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(1), pixelArray.GetLength(0));
            for (int x = 0; x < pixelArray.GetLength(1); x++)
            {
                for (int y = 0; y < pixelArray.GetLength(0); y++)
                {
                    int greyColor = (int) Map(pixelArray[y, x], 0, UInt16.MaxValue, 0, 255);
                    imgBitmap.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                }
            }

            MemoryStream ms = new MemoryStream();
            ApplyOverlays(imgBitmap).Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}