using System;
using System.Drawing;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;

namespace DicomDisplayTest
{
    public class Contrast
    {
        public static Bitmap equalization(DicomImage image)
        {
            var image2 = image;

            // Copying image to int array 
            //int[] pixels = new int[image2.Height * image2.Width];
            int[] new_pixels = new int[image2.Height * image2.Width];
            Color[] pixels = new Color[image2.Height * image2.Width];
            DicomPixelData test;
            
            for (int i = 0; i < image2.Height; i++)
            {
                for (int j = 0; j < image2.Width; j++)
                {
                    pixels[image2.Width*i + j] = image2.RenderImage().AsClonedBitmap().GetPixel(j,i);
                    test = image2.RenderImage().Pixels;
                }
            }
            
            int[] cdf = new int[256];
            int val;
            for (int i = 0; i < image2.Height; i++)
            {
                for (int j = 0; j < image2.Width; j++)
                {
                    val = pixels[image2.Width*i + j].R;
                    cdf[val]++;
                }
            }
            
            Console.WriteLine("waypoint");
            // Finding the minimum brightness
            
            
            int cdfMin = cdf[0];
            for(int i = 1; i < 256; i++) {
                cdf[i] += cdf[i-1];
                if(cdf[i] < cdfMin) {
                    cdfMin = cdf[i];
                }
            }
            
            // Change pixels array
            /*for(int y = 0; y < image2.Height; y++) {
                for(int x = 0; x < image2.Width; x++) {
                    new_pixels[image2.Width*y + x] = Convert.ToInt32((Math.Round((double) cdf[pixels[image2.Width*y + x]] - cdf_min)*255.0/(image2.Width*image2.Height-cdf_min)));
                }
            }*/
            
            for(int y = 0; y < image2.Height; y++) {
                for(int x = 0; x < image2.Width; x++)
                {
                    Console.WriteLine(pixels[y+x]);
                    new_pixels[image2.Width*y + x] = Convert.ToInt32((Math.Round((double) cdf[pixels[image2.Width*y + x].R] - cdfMin)*255.0/(image2.Width*image2.Height-cdfMin)));
                }
            }

            var bitmap = new Bitmap(image.Width, image.Height);
            for(int y = 0; y < image2.Height; y++) {
                for(int x = 0; x < image2.Width; x++)
                {
                    bitmap.SetPixel(x,y,Color.FromArgb(new_pixels[image2.Width*y+x],new_pixels[image2.Width*y+x],new_pixels[image2.Width*y+x])); //= Convert.ToInt32((Math.Round((double) cdf[pixels[image2.Width*y + x].R] - cdfMin)*255.0/(image2.Width*image2.Height-cdfMin)));
                }
            }
            
            // Trying to make a grayscale bitmap insted of RGB
            //bitmap.SetPixel(x,y, Color.FromArg(new_pixels[x], new_pixels[x], new_pixels[x]));
            
            return bitmap;
        }
    }
}