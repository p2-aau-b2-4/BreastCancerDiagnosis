using System;

namespace ImagePreprocessing
{
    public static class Contrast
    {
        // We found formula for making the histogram equalization here: https://epochabuse.com/histogram-equalization/
        public static UShortArrayAsImage ApplyHistogramEqualization(UShortArrayAsImage img)
        {
            int[] histogram = MakeHistogram(img, out int blackPixelsCount);

            double[] normalizedHistogram = MakeNormalizedHistogram(histogram, img.PixelArray.Length - blackPixelsCount);

            double[] accumulativeHistogram = MakeAccumulativeHistogram(normalizedHistogram);

            return new UShortArrayAsImage(CalculateResult(img.PixelArray, accumulativeHistogram));
        }

        private static int[] MakeHistogram(UShortArrayAsImage img, out int blackPixelsCount)
        {
            int[] Histogram = new int[UInt16.MaxValue + 1];
            var pixelArray = img.PixelArray;

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Histogram[pixelArray[i, j]]++;
                }
            }
            //Saving the amount of black pixels and setting the amount of black pixels to zero.
            blackPixelsCount = Histogram[0];
            Histogram[0] = 0;

            return Histogram;
        }

        private static double[] MakeNormalizedHistogram(int[] histogram, int nPixels)
        {
            double[] normalizedHistogram = new double[UInt16.MaxValue + 1];

            for (int i = 0; i < UInt16.MaxValue + 1; i++)
            {
                normalizedHistogram[i] = histogram[i] / (double)nPixels;
            }
            return normalizedHistogram;
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

        private static ushort[,] CalculateResult(ushort[,] origin, double[] accumulativeHistogram)
        {
            ushort[,] result = origin.Clone() as ushort[,];
            for (int i = 0; i < origin.GetLength(0); i++)
            {
                for (int j = 0; j < origin.GetLength(1); j++)
                {
                    result[i, j] = (ushort)(accumulativeHistogram[origin[i, j]] * (double)UInt16.MaxValue);
                }
            }
            return result;
        }
    }
}