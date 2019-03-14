using System;
using System.Drawing;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;

namespace DicomDisplayTest
{
    public class Contrast
    {
        public static ushort[,] equalization(ushort[,] image)
        //public static Bitmap equalization(ushort[,] image)
        {
            
            // Copying image to int array 
            //int[] pixels = new int[image2.Height * image2.Width];
            ushort[,] new_pixels = new ushort[image.GetLength(0), image.GetLength(1)];
            
            
            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    new_pixels[x,y] = image[x,y];
                }
            }
            
            ushort[] cdf = new ushort[256];
            ushort val;
            for (int y = 0; y < image.GetLength(1); y++)
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    val = new_pixels[x,y];
                    cdf[val]++;
                }
            }
            
            Console.WriteLine("waypoint");
            // Finding the minimum brightness
            
            
            ushort cdfMin = cdf[0];
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
            
            for(int y = 0; y < image.GetLength(1); y++) 
            {
                for(int x = 0; x < image.GetLength(0); x++)
                {
                    new_pixels[x,y] = Convert.ToUInt16((Math.Round((double) cdf[new_pixels[x,y]] - cdfMin)*255.0/(image.GetLength(0)*image.GetLength(1)-cdfMin)));
                }
            }

            var bitmap = new Bitmap(image.GetLength(0), image.GetLength(1));
            for(int y = 0; y < image.GetLength(1); y++) {
                for(int x = 0; x < image.GetLength(0); x++)
                {
                    bitmap.SetPixel(x,y,Color.FromArgb(new_pixels[x,y],new_pixels[x,y],new_pixels[x,y]));
                }
            }
            
            // Trying to make a grayscale bitmap insted of RGB
            //bitmap.SetPixel(x,y, Color.FromArg(new_pixels[x], new_pixels[x], new_pixels[x]));
            
            return new_pixels;
        }
    }
}