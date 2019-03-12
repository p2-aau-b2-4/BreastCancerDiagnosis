using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DicomDisplayTest
{
    public class UshortImageInfo
    {
        // noget med kræftdiagnose properties her, som indlæses af csv filer..

        public ushort[,] PixelArray { get; set; }

        private ArrayList _overlays;
        public ArrayList Overlays => _overlays;

        public void AddOverlay(Bitmap overlay)
        {
            _overlays.Add(overlay);
        }

        public UshortImageInfo(ushort[,] pixelArray)
        {
            this.PixelArray = pixelArray;
            this._overlays = new ArrayList();
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
            
            // lets add all bitmaps:
            Graphics g = Graphics.FromImage(imgBitmap);
            g.CompositingMode = CompositingMode.SourceOver;
            foreach (Bitmap bitmap in Overlays)
            {
                bitmap.MakeTransparent();
                g.DrawImage(bitmap, new Point(0,0));
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