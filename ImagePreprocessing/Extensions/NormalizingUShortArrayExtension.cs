using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

<<<<<<< HEAD

            //Console.WriteLine(imageOverlay[(imageOverlay.GetLength(0)-1)/2,imageOverlay.GetLength(1)/2]);
            var x = new UshortArrayAsImage(new byte[ushortImg.Width * ushortImg.Height * 2], ushortImg.Width,
                ushortImg.Height);
            x.PixelArray = GetBiggestSector(imageOverlay, image);
            //TumorDimensions(imageOverlay);
            return x;
        }

        private static int GetMaxD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd)
        {
            int max = int.MinValue;
            for (int i = 0; i < templates.Count; i++)
            {
                int newVal = GetD(cr, cl, cu, cd, cld, clu, cru, crd, templates[i]);
                if (newVal > max)
                    max = newVal;
            }

            return max;
        }

        private static int GetD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd, int[,] matrix)
        {
            return Math.Abs(matrix[0, 0] * clu + matrix[0, 1] * cu + matrix[0, 2] * cru
                            + matrix[1, 0] * cl + matrix[1, 2] * cr
                            + matrix[2, 0] * cld + matrix[2, 1] * cd + matrix[2, 2] * crd);
        }

        private static List<int[,]> templates = new List<int[,]>
        {
            new int[,] {{-3, -3, 5}, {-3, 0, 5}, {-3, -3, 5}},
            new int[,] {{-3, 5, 5}, {-3, 0, 5}, {-3, -3, -3}},
            new int[,] {{5, 5, 5}, {-3, 0, -3}, {-3, -3, -3}},
            new int[,] {{5, 5, -3}, {5, 0, -3}, {-3, -3, -3}},
            new int[,] {{5, -3, -3}, {5, 0, -3}, {5, -3, -3}},
            new int[,] {{-3, -3, -3}, {5, 0, -3}, {5, 5, -3}},
            new int[,] {{-3, -3, -3}, {-3, 0, -3}, {5, 5, 5}},
            new int[,] {{-3, -3, -3}, {-3, 0, 5}, {-3, 5, 5}}
        };

        private static double EuclideanDistance(int p1, int q1, int p2, int q2)
        {
            return Math.Sqrt(Math.Pow(q1 - p1, 2) + Math.Pow(q2 - p2, 2));
            ;
        }

        private static ushort[,] GetBiggestSector(ushort[,] imageOverlayIn, ushort[,] origin)
        {
            List<Sector> sectors = new List<Sector>();
            ushort[,] imageOverlay = imageOverlayIn.Clone() as ushort[,];

            for (int i = 0; i < imageOverlay.GetLength(0); i++)
=======
                if (top == -1 && containsMask) top = y;
                if (top != -1 && bottom == -1 && !containsMask) bottom = y;
            }

            for (int x = 0; x < mask.GetLength(1); x++)
>>>>>>> 82d8424035632281d96f3d262a96ee49c7bd01dc
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