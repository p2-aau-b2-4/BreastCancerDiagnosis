using System;

namespace ImagePreprocessing
{
    public static class Contrast
    {
        public static UShortArrayAsImage ApplyHistogramEqualization(UShortArrayAsImage img)
        {
            int UInt16ValuesInTotal = UInt16.MaxValue + 1;
            int[] histogram = new int[UInt16ValuesInTotal];
            var pixelArray = img.PixelArray;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    histogram[pixelArray[i, j]]++;
                }
            }

            var nPixels = img.PixelArray.Length;
            double[] normalizedHistogram = new double[UInt16ValuesInTotal];
            for (int i = 0; i < UInt16ValuesInTotal; i++)
            {
                normalizedHistogram[i] = histogram[i] / (double) nPixels;
            }


            double[] accumulativeHistogram = MakeAccumulativeHistogram(normalizedHistogram);

            return new UShortArrayAsImage(CalculateResult(img.PixelArray, accumulativeHistogram));
        }

        private static ushort[,] CalculateResult(ushort[,] origin, double[] accumulativeHistogram)
        {
            ushort[,] result = origin.Clone() as ushort[,];
            for (int i = 0; i < origin.GetLength(0); i++)
            {
                for (int j = 0; j < origin.GetLength(1); j++)
                {
                    result[i, j] = (ushort) (accumulativeHistogram[origin[i, j]] * (double) UInt16.MaxValue);
                }
            }

            return result;
        }

        private static double[] MakeAccumulativeHistogram(double[] normalizedHistogram)
        {
            double[] accumulativeHistogram = new double[UInt16.MaxValue + 1];
            double accumulated = 0;
            for (int i = 0; i < UInt16.MaxValue + 1; i++)
            {
                accumulated += normalizedHistogram[i];
                accumulativeHistogram[i] = accumulated;
            }

            return accumulativeHistogram;
        }
    }
}