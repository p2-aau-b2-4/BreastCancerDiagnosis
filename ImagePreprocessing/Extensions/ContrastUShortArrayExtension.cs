using System;

namespace ImagePreprocessing
{
    public static class UShortArrayAsImageImageProcessingExtensions
    {
        public static void ApplyContrastEnhancement(this UshortArrayAsImage img, double threshold)
        {
            ushort[,] image = img.PixelArray;

            double maxUInt16Value = UInt16.MaxValue;
            // Copying image to int array 
            //int[] pixels = new int[image2.Height * image2.Width];
            ushort[,] pixels = new ushort[image.GetLength(0), image.GetLength(1)];
            ushort[,] endPixels = new ushort[image.GetLength(0), image.GetLength(1)];


            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    pixels[x, y] = image[x, y];
                }
            }

            //double threshold = 50.0;
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double pixelToInsert = 0.0;


            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    pixelToInsert = ApplyContrastToPixel(pixels[x, y], maxUInt16Value, contrastLevel);

                    if (pixelToInsert > maxUInt16Value)
                    {
                        pixelToInsert = maxUInt16Value;
                    }
                    else if (pixelToInsert < 0)
                    {
                        pixelToInsert = 0;
                    }

                    endPixels[x, y] = Convert.ToUInt16(pixelToInsert);
                }
            }

            img.PixelArray = endPixels;
        }

        private static double ApplyContrastToPixel(ushort pixelToTransform, double maxValue, double contrastLevel)
        {
            return ((((pixelToTransform / maxValue) - 0.5) * contrastLevel) + 0.5) * maxValue;
        }

        public static long CalulateAverage(this UshortArrayAsImage img)
        {
            // Calculate average


            ushort[,] image = img.PixelArray;

            long accumulator = 0;
            long counter = 0;

            for (int a = 0; a < image.GetLength(1); a++)
            {
                for (int b = 0; b < image.GetLength(0); b++)
                {
                    if (image[b, a] > 100)
                    {
                        accumulator += image[b, a];
                        //Console.WriteLine(accumulator);
                        counter++;
                    }
                }
            }

            long average = accumulator / counter;

            Console.WriteLine(average);

            return average;
        }


        // We found formula for making the histogram equalization here: https://epochabuse.com/histogram-equalization/
        public static void ApplyHistogramEqualization(this UshortArrayAsImage img)
        {
            int[] histogram = MakeHistogram(img);
            
            double[] normalizedHistogram = MakeNormalizedHistogram(histogram, img.PixelArray.Length);

            double[] accumulativeHistogram = MakeAccumulativeHistogram(normalizedHistogram);

            img.PixelArray = CalculateResult(img.PixelArray, accumulativeHistogram);
        }

        private static int[] MakeHistogram(UshortArrayAsImage img)
        {
            int[] Histogram = new int[UInt16.MaxValue + 1];
            var pixelArray = img.PixelArray;

            for (int j = 0; j < img.Height; j++)
            {
                for (int i = 0; i < img.Width; i++)
                {
                    Histogram[pixelArray[i, j]]++;
                }
            }
            return Histogram;
        }

        private static double[] MakeNormalizedHistogram(int[] histogram, int totalPixels)
        {
            double[] normalizedHistogram = new double[UInt16.MaxValue + 1];

            for (int i = 0; i < UInt16.MaxValue + 1; i++)
            {
                normalizedHistogram[i] = histogram[i] / (double)totalPixels;
            }
            return normalizedHistogram;
        }


        private static double[] MakeAccumulativeHistogram(double[] normalizedHistogram)
        {
            double[] accumulativeHistogram = new double[UInt16.MaxValue+1];
            double accumulated = 0;
            for (int i = 0; i < UInt16.MaxValue+1; i++)
            {
                accumulated += normalizedHistogram[i];
                accumulativeHistogram[i] = accumulated;
            }

            return accumulativeHistogram;
        }

        private static ushort[,] CalculateResult(ushort[,] origin, double[] accumulativeHistogram)
        {
            ushort[,] result = origin.Clone() as ushort[,];
            for (int j = 0; j < origin.GetLength(0); j++)
            {
                for (int i = 0; i < origin.GetLength(1); i++)
                {
                    result[i, j] = (ushort)(accumulativeHistogram[origin[i, j]] * (double)UInt16.MaxValue);
                }
            }

            return result;
        }
    }
}