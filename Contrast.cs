using System;
using System.Drawing;
using Dicom.Imaging;

namespace DicomDisplayTest
{
    public class Contrast
    {
        public static Bitmap equalization(DicomImage image)
        {
            var image2 = image;

            // Copying image to int array 
            int[] pixels = new int[image2.Height * image2.Width];
            int[] new_pixels = new int[image2.Height * image2.Width];
            
            foreach (var VARIABLE in pixels)
            {
                pixels[VARIABLE] = image2.RenderImage().Pixels[VARIABLE];
            }

            // Finding the minimum brightness
            
            int[] cdf = new int[256];
            int cdf_min = cdf[0];
            for(int i = 1; i < 256; i++) {
                cdf[i] += cdf[i-1];
                if(cdf[i] < cdf_min) {
                    cdf_min = cdf[i];
                }
            }
            
            // Change pixels array
            for(int y = 0; y < image2.Height; y++) {
                for(int x = 0; x < image2.Width; x++) {
                    new_pixels[image2.Width*y + x] = Convert.ToInt32((Math.Round((double) cdf[pixels[image2.Width*y + x]] - cdf_min)*255.0/(image2.Width*image2.Height-cdf_min)));
                }
            }

            var bitmap = new Bitmap(image.Width, image.Height);
            // Trying to make a grayscale bitmap insted of RGB
            bitmap.SetPixel(x,y, Color.);
            
            return new_pixels;
        }
    }
}