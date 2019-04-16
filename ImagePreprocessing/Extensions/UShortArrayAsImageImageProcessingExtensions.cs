using System;
using System.Collections.Generic;
using System.Text;

namespace ImagePreprocessing
{
    static class UShortArrayAsImageImageProcessingExtensions
    {
        public static void ApplyContrastEnhancement(this UshortArrayAsImage img)
        {
            double threshold = 50;

            ushort[,] image = img.PixelArray;

            double pixBit = UInt16.MaxValue;
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
            double grey = 0.0;



            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    grey = ((((pixels[x, y] / pixBit) - 0.5) * contrastLevel) + 0.5) * pixBit;

                    if (grey > pixBit)
                    {
                        grey = pixBit;
                    }
                    else if (grey < 0)
                    {
                        grey = 0;
                    }

                    end_pixels[x, y] = Convert.ToUInt16(grey);
                }
            }

            img.PixelArray = end_pixels;



        }
        public static long calulateAverage(this UshortArrayAsImage img)
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
