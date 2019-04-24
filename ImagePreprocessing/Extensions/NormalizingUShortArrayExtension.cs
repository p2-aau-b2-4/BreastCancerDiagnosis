using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.IO;
using Accord.Math;

namespace ImagePreprocessing
{
    public static class NormalizingUShortArrayExtension
    {
        public static UshortArrayAsImage GetNormalizedCrop(this UshortArrayAsImage image, Rectangle tumour, int size,
            int tumourSize)
        {
            // Rectangle = det markerede område med knuden (kan godt være ikke kvadratisk, men så lav det kvadratisk)
            // tumoursize = hvor stor knuden skal være i output (kvadratisk)
            // size = hvor stort billedet skal være (kvadratisk)
            // returns a ushortarrayasimage, with black boxes around the resized tumour.
            throw new NotImplementedException();
        }

        public static Rectangle GetTumourPositionFromMask(this UByteArrayAsImage maskUbyte)
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