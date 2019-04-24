using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImagePreprocessing
{
    public abstract class ArrayAsImageAbstract
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] PixelData { get; set; } // up to class to interpret byte array;

        protected ArrayAsImageAbstract(byte[] pixelData, int width, int height)
        {
            PixelData = pixelData;
            Width = width;
            Height = height;
        }

        private List<Bitmap> _overlays = new List<Bitmap>();
        public List<Bitmap> Overlays => _overlays;

        public void AddOverlay(Bitmap overlay)
        {
            _overlays.Add(overlay);
        }

        public void SaveAsPng(String saveLoc)
        {
            using (FileStream file = new FileStream(saveLoc, FileMode.Create))
            {
                GetPngAsMemoryStream().CopyTo(file);
            }
        }
        public abstract Stream GetPngAsMemoryStream();

        protected static float Map(float s, float a1, float a2, float b1, float b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}