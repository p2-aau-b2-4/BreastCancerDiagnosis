using System;

namespace ImagePreprocessing
{
    public class BilinearInterpoliation
    {
        // this is an attempt to use bilinearinterpolation instead of nearest neighboor to reisze an image:
        //https://en.wikipedia.org/wiki/Bilinear_interpolation


        public static UShortArrayAsImage ResizeImageBilinearInterpolation(UShortArrayAsImage uShortArrayAsImageIn,
            int size)
        {
            var newImage = new ushort[size, size];
            var oldImage = uShortArrayAsImageIn.PixelArray;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    // for every pixel in the new image, do this:
                    double xPos = Normalization.Map(x, 0, size-1, 0, oldImage.GetLength(1)-1);
                    double yPos = Normalization.Map(y, 0, size-1, 0, oldImage.GetLength(0)-1);

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    double xLeftFactor;
                    double xRightFactor;
                    if (xPos == Math.Floor(xPos))
                    {
                        // x is a whole number, and thus represents a whole column.
                        xLeftFactor = 0.5;
                        xRightFactor = 0.5;
                    }
                    else
                    {
                        xLeftFactor = (xPos - Math.Floor(xPos));
                        xRightFactor = (Math.Ceiling(xPos) - xPos);
                    }

                    double xTop =
                        xRightFactor * oldImage[(int) Math.Floor(yPos), (int) Math.Floor(xPos)] +
                        xLeftFactor *oldImage[(int) Math.Floor(yPos), (int) Math.Ceiling(xPos)];
                    double xBottom =
                        xRightFactor * oldImage[(int) Math.Ceiling(yPos), (int) Math.Floor(xPos)] +
                        xLeftFactor * oldImage[(int) Math.Ceiling(yPos), (int) Math.Ceiling(xPos)];

                    double yTopFactor;
                    double yBottomFactor;
                    if (yPos == Math.Floor(yPos))
                    {
                        // x is a whole number, and thus represents a whole column.
                        yTopFactor = 0.5;
                        yBottomFactor = 0.5;
                    }
                    else
                    {
                        yTopFactor = yPos - Math.Floor(yPos);
                        yBottomFactor = (Math.Ceiling(yPos) - yPos);
                    }
                    
                    
                    newImage[y, x] =
                        (ushort) (yBottomFactor * xTop + yTopFactor * xBottom);
                }
            }

            return new UShortArrayAsImage(newImage);
        }
    }
}