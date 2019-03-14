using System;
using System.Drawing;
using System.Xml.Linq;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;

namespace DicomDisplayTest
{
    public class Contrast
    {
        public static ushort[,] Equalization(ushort[,] image, double threshold)
        {
            
            // Copying image to int array 
            //int[] pixels = new int[image2.Height * image2.Width];
            ushort[,] pixels = new ushort[image.GetLength(0), image.GetLength(1)];
            ushort[,] end_pixels = new ushort[image.GetLength(0), image.GetLength(1)];
            
            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    pixels[x,y] = image[x,y];
                }
            }

            //double threshold = 50.0;
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double grey = 0.0;

            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    grey = ((((pixels[x,y] / 65535.0) - 0.5) *  
                             contrastLevel) + 0.5) * 65535.0;
                    
                    if (grey > 65535)
                    {
                        grey = 65535;
                    } 
                    else if (grey < 0)
                    {
                        grey = 0;
                    }

                    end_pixels[x, y] = Convert.ToUInt16(grey);
                }
            }

            return end_pixels;
        }
    }
}