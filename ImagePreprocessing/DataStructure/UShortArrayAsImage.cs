using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImagePreprocessing
{
    public class UShortArrayAsImage : ArrayAsImageAbstract<ushort[,]>
    {
        public sealed override ushort[,] PixelArray
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

        public UShortArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }

        public UShortArrayAsImage(ushort[,] arrayIn) : base(null, arrayIn.GetLength(1), arrayIn.GetLength(0))
        {
            PixelData = new byte[arrayIn.Length*2];
            PixelArray = arrayIn;
        }

        public override Stream GetPngAsMemoryStream()
        {
            var pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(1), pixelArray.GetLength(0));
            BitmapData imgBitmapData = imgBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height),
            ImageLockMode.ReadWrite, imgBitmap.PixelFormat);
            IntPtr scan0 = imgBitmapData.Scan0;

            int bytes = imgBitmapData.Height * Math.Abs(imgBitmapData.Stride);
            byte[] byteArray = new byte[bytes];
            Marshal.Copy(scan0, byteArray, 0, bytes);

            int position = 0;
            for (int y = 0; y < pixelArray.GetLength(0); y++)
            {
                for (int x = 0; x < pixelArray.GetLength(1); x++)
                {
                    byte greyColor = (byte)Math.Round(Map(pixelArray[y, x], 0, UInt16.MaxValue, 0, 255));
                    byteArray[position++] = greyColor;
                    byteArray[position++] = greyColor;
                    byteArray[position++] = greyColor;
                    byteArray[position++] = 255;
                }
            }

            Marshal.Copy(byteArray, 0, scan0, bytes);

            MemoryStream ms = new MemoryStream();
            ApplyOverlays(imgBitmap).Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public void SaveAsPng2(String saveLoc)
        {
            using (FileStream file = new FileStream(saveLoc, FileMode.Create))
            {
                GetPngAsMemoryStream().CopyTo(file);
            }
        }
    }
}
