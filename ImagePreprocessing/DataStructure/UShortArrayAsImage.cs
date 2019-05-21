using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
                ushort[,] result = new ushort[Height, Width];
                // this uses blockCopy, since data format is the same in byte[] and ushort[,]
                Buffer.BlockCopy(PixelData, 0, result, 0, PixelData.Length);
                return result;
            }
            set
            {
                byte[] result = new byte[Width * Height * 2];
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
        
        /// <summary>
        /// Creates a memory stream from a bitmap, and uses it to save it as PNG.
        /// </summary>
        /// <returns> The memory stream for the PNG </returns>
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
    }
}
