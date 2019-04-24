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

        /*public UshortArrayAsImage Crop((int, int, int, int) rectangle)
        {
            //todo
            int x1 = rectangle.Item1 < rectangle.Item3 ? rectangle.Item1 : rectangle.Item3;
            int x2 = rectangle.Item1 > rectangle.Item3 ? rectangle.Item1 : rectangle.Item3;
            int y1 = rectangle.Item2 < rectangle.Item4 ? rectangle.Item2 : rectangle.Item4;
            int y2 = rectangle.Item2 > rectangle.Item4 ? rectangle.Item2 : rectangle.Item4;
            Console.WriteLine($"{rectangle.Item1},{rectangle.Item2},{rectangle.Item3},{rectangle.Item4}");
            Console.WriteLine($"{x1},{y1},{x2},{y2}");
            
            int sizeX = x2 - x1;
            int sizeY = y2 - y1;

            Console.WriteLine($"SIZE: {sizeX}X{sizeY}");
            Console.WriteLine($"SIZEOriginal: {Width}X{Height}");
            
            
            ushort[,] result = new ushort[sizeY,sizeX];
            

            ushort[,] current = PixelArray; // set here - lazy evaluation
            
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)

                {
                    result[y, x] = current[y + y1,x + x1];
                }
            }
            
            byte[] resultBytes = new byte[sizeX*sizeY * 2];
            Buffer.BlockCopy(result, 0, resultBytes, 0, resultBytes.Length);
            var finalResult = new UshortArrayAsImage(resultBytes,sizeX,sizeY);
            return finalResult;
        }

        public void CropFromMask(UByteArrayAsImage getDcomMaskImage, string appSetting)
        {
         }*/
    }
}