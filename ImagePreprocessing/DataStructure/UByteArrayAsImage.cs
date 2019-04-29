using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using CSJ2K.j2k.entropy.decoder;
using Dicom.IO;

namespace ImagePreprocessing
{
    public class UByteArrayAsImage : ArrayAsImageAbstract<byte[,]>
    {
        public sealed override byte[,] PixelArray
        {
            get
            {
                byte[,] result = new byte[this.Width, this.Height];
                // this uses blockcopy, since data format is the same in byte[] and byte[,]
                Buffer.BlockCopy(PixelData, 0, result, 0, PixelData.Length);
                return result;
            }
            set
            {
                byte[] result = new byte[this.Width * this.Height];
                Buffer.BlockCopy(value, 0, result, 0, PixelData.Length);
                PixelData = result;
            }
        }

        public UByteArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }
        public UByteArrayAsImage(byte[,] arrayIn) : base(null, arrayIn.GetLength(1), arrayIn.GetLength(0))
        {
            PixelData = new byte[arrayIn.Length];
            PixelArray = arrayIn;
        }

        public override Stream GetPngAsMemoryStream()
        {
            byte[,] pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(0), pixelArray.GetLength(1));
            BitmapData imgBitmapData = imgBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height),
            ImageLockMode.ReadWrite, imgBitmap.PixelFormat);
            IntPtr scan0 = imgBitmapData.Scan0;

            int bytes = imgBitmapData.Height * Math.Abs(imgBitmapData.Stride);
            byte[] byteArray = new byte[bytes];
            int[] intArray = new int[bytes / 4];
            Marshal.Copy(scan0, byteArray, 0, bytes);


            int position = 0;
            for (int x = 0; x < pixelArray.GetLength(0); x++)
            {
                for (int y = 0; y < pixelArray.GetLength(1); y++)
                {
                    byte greyColor = pixelArray[x, y];
                    byteArray[position++] = greyColor;
                    byteArray[position++] = greyColor;
                    byteArray[position++] = greyColor;
                    byteArray[position++] = 255;
                }
            }
            Marshal.Copy(byteArray, 0, scan0, bytes);

            imgBitmap.UnlockBits(imgBitmapData);
            MemoryStream ms = new MemoryStream();
            ApplyOverlays(imgBitmap).Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override void SaveAsPng(String saveLoc)
        {
            using (FileStream file = new FileStream(saveLoc, FileMode.Create))
            {
                GetPngAsMemoryStream().CopyTo(file);
            }
        }
    }
}
