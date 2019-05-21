using System;

namespace ImagePreprocessing
{
    /// <summary>
    /// Class for adding histogram equalization to an image. 
    /// </summary>
    public static class Contrast
    {
        /// <summary>
        /// Applies histogram equalization to a given image.
        /// </summary>
        /// <param name="img"> The image of which to apply histogram equalization to </param>
        /// <returns> The image with applied histogram equalization </returns>
        public static UShortArrayAsImage ApplyHistogramEqualization(UShortArrayAsImage img)
            // We found formula for making the histogram equalization here: https://epochabuse.com/histogram-equalization/
        {
            int[] histogram = MakeHistogram(img, out int blackPixelsCount);

            double[] normalizedHistogram = MakeNormalizedHistogram(histogram, img.PixelArray.Length - blackPixelsCount);

            double[] accumulativeHistogram = MakeAccumulativeHistogram(normalizedHistogram);

            return new UShortArrayAsImage(CalculateResult(img.PixelArray, accumulativeHistogram));
        }
        
        /// <summary>
        /// Creates a histogram based on the given image.
        /// </summary>
        /// <param name="img"> The image </param>
        /// <param name="blackPixelsCount"> The amount of black pixels in the picture </param>
        /// <returns> The image histogram </returns>
        private static int[] MakeHistogram(UShortArrayAsImage img, out int blackPixelsCount)
        {
            int[] histogram = new int[UInt16.MaxValue + 1];
            var pixelArray = img.PixelArray;

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    histogram[pixelArray[i, j]]++;
                }
            }
            //Saving the amount of black pixels and setting the amount of black pixels to zero.
            blackPixelsCount = histogram[0];
            histogram[0] = 0;

            return histogram;
        }

        /// <summary>
        /// Normalizes the histogram created previously.
        /// </summary>
        /// <param name="histogram"> The image histogram </param>
        /// <param name="nPixels"> Total amount of pixels in the image, not counting the black </param>
        /// <returns> The normalized histogram </returns>
        private static double[] MakeNormalizedHistogram(int[] histogram, int nPixels)
        {
            double[] normalizedHistogram = new double[UInt16.MaxValue + 1];

            for (int i = 0; i < UInt16.MaxValue + 1; i++)
            {
                normalizedHistogram[i] = histogram[i] / (double)nPixels;
            }
            return normalizedHistogram;
        }

        /// <summary>
        /// Calculates the accumulative histogram, based on the normalized histogram.
        /// </summary>
        /// <param name="normalizedHistogram"> The normalized histogram </param>
        /// <returns> Array containing the sum of all the values in the normalized histogram </returns>
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
        
        /// <summary>
        /// Calculates the final result of the histogram equalization. 
        /// </summary>
        /// <param name="origin">The original image</param>
        /// <param name="accumulativeHistogram">The output from "MakeAccumulativeHistogram"</param>
        /// <returns>Returns the finished image as a 2D ushort array</returns>
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