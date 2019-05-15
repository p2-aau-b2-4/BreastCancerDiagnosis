using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImagePreprocessing
{
    public class UByteArrayAsImage : ArrayAsImageAbstract<byte[,]>
    {
        public sealed override byte[,] PixelArray
        {
            get
            {
                byte[,] result = new byte[Height, Width];
                // this uses blockcopy, since data format is the same in byte[] and byte[,]
                Buffer.BlockCopy(PixelData, 0, result, 0, PixelData.Length);
                return result;
            }
            set
            {
                byte[] result = new byte[Height * Width];
                Buffer.BlockCopy(value, 0, result, 0, PixelData.Length);
                PixelData = result;
            }
        }

        public UByteArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }

        public UByteArrayAsImage(byte[,] arrayIn) : base(new byte[arrayIn.Length], arrayIn.GetLength(1),
            arrayIn.GetLength(0))
        {
            PixelArray = arrayIn;
        }

        public override Stream GetPngAsMemoryStream()
        {
            byte[,] pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(1), pixelArray.GetLength(0));
            BitmapData imgBitmapData = imgBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height),
                ImageLockMode.ReadWrite, imgBitmap.PixelFormat);
            unsafe
            {
                byte* byteArray = (byte*) imgBitmapData.Scan0.ToPointer();
                int position = 0;

                for (int y = 0; y < pixelArray.GetLength(0); y++)
                {
                    for (int x = 0; x < pixelArray.GetLength(1); x++)
                    {
                        byte greyColor = pixelArray[y, x];
                        byteArray[position++] = greyColor;
                        byteArray[position++] = greyColor;
                        byteArray[position++] = greyColor;
                        byteArray[position++] = 255;
                    }
                }
            }

            imgBitmap.UnlockBits(imgBitmapData);
            MemoryStream ms = new MemoryStream();
            ApplyOverlays(imgBitmap).Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}