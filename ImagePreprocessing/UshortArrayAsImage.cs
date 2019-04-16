using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImagePreprocessing
{
    public class UshortArrayAsImage : ArrayAsImageAbstract
    {
//        private ushort[,] _pixelArray;

        public ushort[,] PixelArray
        {
            get
            {
                ushort[,] result = new ushort[this.Height,this.Width];
                // this uses blockcopy, since data format is the same in byte[] and ushort[,]
                Buffer.BlockCopy(PixelData, 0, result, 0, PixelData.Length);
                return result;
            }
            set
            {
                byte[] result = new byte[this.Width * this.Height * 2];
                Buffer.BlockCopy(value,0,result,0,PixelData.Length);
                PixelData = result;
            }
        }

        /*public void ApplyNoiseFilter(int radius)
        {
            ushort[,] tempMatrix = new ushort[PixelArray.GetLength(0), PixelArray.GetLength(1)];
            for (int x = 0; x < tempMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < tempMatrix.GetLength(1); y++)
                {
                    //lets add all pixels and average them
                    int count = 0;
                    int total = 0;
                    for (int xin = x - radius; xin < x + radius; xin++)
                    {
                        if (xin < 0 || xin >= tempMatrix.GetLength(0)) continue;
                        for (int yin = y - radius; yin < y + radius; yin++)
                        {
                            if (yin < 0 || yin >= tempMatrix.GetLength(1)) continue;
                            count++;
                            total += PixelArray[xin, yin];
                        }
                    }

                    tempMatrix[x, y] = Convert.ToUInt16(total * 1.0 / count);
                }
            }

            PixelArray = tempMatrix;
        }*/

        public override void SaveAsPng(string saveLoc)
        {
            var pixelArray = PixelArray;
            Bitmap imgBitmap = new Bitmap(pixelArray.GetLength(1), pixelArray.GetLength(0));
            for (int x = 0; x < pixelArray.GetLength(1); x++)
            {
                for (int y = 0; y < pixelArray.GetLength(0); y++)
                {
                    int greyColor = (int) Map(pixelArray[y, x], 0, 65535, 0, 255);
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

            imgBitmap.Save(saveLoc, ImageFormat.Png);
        }

        public UshortArrayAsImage(byte[] pixelData, int width, int height) : base(pixelData, width, height)
        {
        }
    }
}