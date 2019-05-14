using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImagePreprocessing
{
    [Serializable]
    public class UShortArrayAsImage : ArrayAsImageAbstract<ushort[,]>
    {
        /// <summary>
        /// A 2D ushort array of an image.
        /// </summary>
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

        public UShortArrayAsImage(ushort[,] arrayIn) : base(new byte[arrayIn.Length*2], arrayIn.GetLength(1), arrayIn.GetLength(0))
        {
            PixelArray = arrayIn; 
        }

        public override Stream GetPngAsMemoryStream()
        {
            var pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(1), pixelArray.GetLength(0));
            BitmapData imgBitmapData = imgBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height),
            ImageLockMode.ReadWrite, imgBitmap.PixelFormat);
            unsafe
            {
                byte* bitmapDataPointer = (byte*) imgBitmapData.Scan0.ToPointer();
                int position = 0;
                for (int y = 0; y < pixelArray.GetLength(0); y++)
                {
                    for (int x = 0; x < pixelArray.GetLength(1); x++)
                    {
                        byte greyColor = (byte) (pixelArray[y,x] * 255.0 / UInt16.MaxValue);
                        bitmapDataPointer[position++] = greyColor;
                        bitmapDataPointer[position++] = greyColor;
                        bitmapDataPointer[position++] = greyColor;
                        bitmapDataPointer[position++] = 255;
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            ApplyOverlays(imgBitmap).Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        protected static float Map(float s, float a1, float a2, float b1, float b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}
