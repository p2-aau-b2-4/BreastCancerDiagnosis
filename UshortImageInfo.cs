using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DicomDisplayTest
{
    public class UshortImageInfo
    {
        // noget med kræftdiagnose properties her, som indlæses af csv filer..

        public ushort[,] PixelArray { get; set; }

        public UshortImageInfo(ushort[,] pixelArray)
        {
            this.PixelArray = pixelArray;
        }
        public void Render(String saveLoc)
        {
            Bitmap imgBitmap = new Bitmap(PixelArray.GetLength(0),PixelArray.GetLength(1));
            for (int x = 0; x < PixelArray.GetLength(0); x++)
            {
                for (int y = 0; y < PixelArray.GetLength(1); y++)
                {
                    int greyColor = (int) map(PixelArray[x, y],0,65535,0,255);
                    imgBitmap.SetPixel(x,y,Color.FromArgb(greyColor,greyColor,greyColor));
                }
            }
            imgBitmap.Save(saveLoc,ImageFormat.Png);
        }
        
        private static float map(float s, float a1, float a2, float b1, float b2)
        // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);
        }
    }
}