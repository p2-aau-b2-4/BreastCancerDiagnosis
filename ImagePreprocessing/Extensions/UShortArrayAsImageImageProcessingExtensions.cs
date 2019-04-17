using System;
using System.Collections.Generic;
using System.Text;

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
            ushort[,] end_pixels = new ushort[image.GetLength(0), image.GetLength(1)];



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

                    end_pixels[x, y] = Convert.ToUInt16(pixelToInsert);
                }
            }
            img.PixelArray = end_pixels;
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
    }
}
