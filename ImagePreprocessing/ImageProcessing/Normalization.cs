using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImagePreprocessing
{
    public static class Normalization
    {
        public static UShortArrayAsImage GetNormalizedImage(UShortArrayAsImage image, Rectangle tumour, int size,
            int tumourSize)
        {
            // Rectangle = det markerede område med knuden (kan godt være ikke kvadratisk, men så lav det kvadratisk)
            // tumoursize = hvor stor knuden skal være i output (kvadratisk)
            // size = hvor stort billedet skal være (kvadratisk)
            // returns a ushortarrayasimage, with black boxes around the resized tumour.
            throw new NotImplementedException();
        }
        
        private static ushort FindNearest(double x, double y, ushort[,] image)
        {
            return image[ (int) y,(int)x];
        }

        private static float Map(float s, float a1, float a2, float b1, float b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            //todo denne kode er flere steder
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public static UShortArrayAsImage ResizeImage(UShortArrayAsImage uShortArrayAsImageIn, int resizeWidth, int resizeHeight)
        {
            var newImage = new ushort[ resizeHeight,resizeWidth];
            var image = uShortArrayAsImageIn.PixelArray;

            for (int x = 0; x < resizeWidth; x++)
            {
                for (int y = 0; y < resizeHeight; y++)
                {
                    newImage[y, x] = FindNearest(Map(x, 0, resizeWidth, 0, image.GetLength(1)),
                        Map(y, 0, resizeHeight, 0, image.GetLength(0)), image);
                }
            }
            return new UShortArrayAsImage(newImage);
        }

        public static Rectangle GetTumourPositionFromMask(UByteArrayAsImage maskUbyte)
        {
            var mask = maskUbyte.PixelArray;
            int left = -1, right = -1, top = -1, bottom = -1;
            // finding the 4 edges of a rectangle, going from left, top, right and bottom.
            for (int y = 0; y < mask.GetLength(0); y++)
            {
                bool containsMask = false;
                for (int x = 0; x < mask.GetLength(1); x++)
                {
                    if (mask[y, x] != 0)
                    {
                        containsMask = true;
                        break;
                    }
                }

                if (top == -1 && containsMask) top = y;
                if (top != -1 && bottom == -1 && !containsMask) bottom = y;
            }

            for (int x = 0; x < mask.GetLength(1); x++)
            {
                bool containsMask = false;
                for (int y = 0; y < mask.GetLength(0); y++)
                {
                    if (mask[y, x] != 0)
                    {
                        containsMask = true;
                        break;
                    }
                }

                if (left == -1 && containsMask) left = x;
                if (left != -1 && right == -1 && !containsMask) right = x;
            }

            return new Rectangle(top, left, bottom - top, right - left);
        }
    }
}