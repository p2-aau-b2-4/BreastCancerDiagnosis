using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImagePreprocessing
{
    public static class Normalization
    {
        public static UShortArrayAsImage GetNormalizedImage(UShortArrayAsImage image, Rectangle tumour, int size)
        {

            Rectangle squareTumour = new Rectangle();
            
            //Resize the Rectangle 
            if (tumour.Width > tumour.Height)
            {
                squareTumour = new Rectangle(tumour.X, tumour.Y - (tumour.Width-tumour.Height) / 2, tumour.Width, tumour.Width);   
            }   
            else if (tumour.Width < tumour.Height || tumour.Width == tumour.Height)
            {
                squareTumour = new Rectangle(tumour.X - (tumour.Height-tumour.Width)/2, tumour.Y, tumour.Height, tumour.Height);
            }
            
            return ResizeImage(Crop(squareTumour, image), size);


        }

        public static ushort FindNearest(double x, double y, ushort[,] image)
        {
            return image[(int)y, (int)x];
        }

        public static float Map(float s, float a1, float a2, float b1, float b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            //todo denne kode er flere steder
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
        
        /// <summary>
        /// Resizes the given image to the given size.
        /// </summary>
        /// <param name="uShortArrayAsImageIn"> Image to be resized </param>
        /// <param name="size"> Width and height of the resized image </param>
        /// <returns> The resized image </returns>
        public static UShortArrayAsImage ResizeImage(UShortArrayAsImage uShortArrayAsImageIn, int size)
        {
            var newImage = new ushort[size, size];
            var image = uShortArrayAsImageIn.PixelArray;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    newImage[y, x] = FindNearest(Map(x, 0, size, 0, image.GetLength(1)),
                        Map(y, 0, size, 0, image.GetLength(0)), image);
                }
            }
            return new UShortArrayAsImage(newImage);
        }

        public static Rectangle GetTumourPositionFromMask(UByteArrayAsImage maskUbyte)
        {
            byte[,] mask = maskUbyte.PixelArray;
            int left = -1, right = -1, top = -1, bottom = -1;
            // finding the 4 edges of a rectangle, going from left, top, right and bottom.
            for (int y = 0; y < mask.GetLength(0); y++)
            {

                bool containsMask = false;
                for (int x = 0; x < mask.GetLength(1); x++)
                {
                    // Console.WriteLine($"{x},{y} = {mask[x,y]}");
                    if (mask[y, x] == 255)
                    {
                        containsMask = true;
                        break;

                    }
                }

                if (top == -1 && containsMask) top = y;
                if(containsMask) bottom = y+1;
            }

            for (int x = 0; x < mask.GetLength(1); x++)
            {
                bool containsMask = false;
                for (int y = 0; y < mask.GetLength(0); y++)
                {
                    if (mask[y, x] == 255)
                    {
                        containsMask = true;
                        break;
                    }
                }

                if (left == -1 && containsMask) left = x;
                if(containsMask) right = x+1;
            }

            if (right == -1) right = mask.GetLength(1)-1;
            if (bottom == -1) bottom = mask.GetLength(0)-1;

            return new Rectangle(left, top, right-left, bottom-top);
        }
        
        public static UShortArrayAsImage Crop(Rectangle rectangle, UShortArrayAsImage image)
        {
            ushort[,] result = new ushort[rectangle.Height,rectangle.Width];

            ushort[,] current = image.PixelArray; // set here - lazy evaluation

            for (int x = 0; x < rectangle.Width; x++)
            {
                if (rectangle.X + x < 0 || rectangle.X + x >= image.Width) continue;
                for (int y = 0; y < rectangle.Height; y++)
                {
                    if (rectangle.Y + y < 0 || rectangle.Y+y >= image.Height) continue;
                    result[y, x] = current[y + rectangle.Y,x + rectangle.X];
                }
            }
            return new UShortArrayAsImage(result);
        }
    }
}