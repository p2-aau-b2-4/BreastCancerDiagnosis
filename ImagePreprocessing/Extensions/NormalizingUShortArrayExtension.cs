using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Accord.IO;
using Accord.Math;

namespace ImagePreprocessing
{
    public static class NormalizingUShortArrayExtension
    {
        public static UshortArrayAsImage GetNormalizedCrop(this UshortArrayAsImage image, Rectangle tumour, int size, int tumourSize)
        {
            // Rectangle = det markerede område med knuden (kan godt være ikke kvadratisk, men så lav det kvadratisk)
            // tumoursize = hvor stor knuden skal være i output (kvadratisk)
            // size = hvor stort billedet skal være (kvadratisk)
            // returns a ushortarrayasimage, with black boxes around the resized tumour.

            Bitmap imageBit = UshortToBitmap(Crop(tumour, image).PixelArray);
            
            var destRect = new Rectangle(0, 0, tumourSize, tumourSize);
            var destImage = new Bitmap(size, size);

            destImage.SetResolution(imageBit.HorizontalResolution, imageBit.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(imageBit, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            
            throw new NotImplementedException();
        }

        private static Bitmap UshortToBitmap(ushort[,] image)
        {

            
            Bitmap imgBitmap = new Bitmap(image.GetLength(1), image.GetLength(0));
            for (int x = 0; x < image.GetLength(1); x++)
            {
                for (int y = 0; y < image.GetLength(0); y++)
                {
                    int greyColor = (int) Map(image[y, x], 0, 65535, 0, 255);
                    imgBitmap.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                }
            }

            return imgBitmap;
        }
        
        private static UshortArrayAsImage Crop(Rectangle rectangle, UshortArrayAsImage image)
        {
            ushort[,] result = new ushort[rectangle.Height,rectangle.Width];
            
            ushort[,] current = image.PixelArray; // set here - lazy evaluation
            
            for (int x = 0; x < rectangle.Width; x++)
            {
                for (int y = 0; y < rectangle.Height; y++)
                {
                    result[y, x] = current[y + rectangle.Y,x + rectangle.X];
                }
            }
            
            byte[] resultBytes = new byte[rectangle.Width*rectangle.Height * 2];
            Buffer.BlockCopy(result, 0, resultBytes, 0, resultBytes.Length);
            var finalResult = new UshortArrayAsImage(resultBytes,rectangle.Width,rectangle.Height);
            return finalResult;
        }
        
        private static float Map(float s, float a1, float a2, float b1, float b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
        
        public static void GetNormalizedSizedCrop(this DdsmImage ddsmImage, Rectangle tumour, int size, int tumourSize)
        {
            // use mask to find rectangle around crop
            var maskUbyte = ddsmImage.GetDcomMaskImage();
            var mask = ddsmImage.GetDcomMaskImage().PixelArray;
            int left = -1, right = -1, top = -1, bottom = -1;
            // finding the 4 edges of a rectangle, going from left, top, right and bottom.
            for (int y = 0; y < mask.GetLength(0); y++)
            {
                bool containsMask = false;
                for (int x = 0; x < mask.GetLength(1); x++)
                {
                    if (mask[y, x] != 0)
                    {
                        containsMask = true;
                        break;
                    }
                }

                if (top == -1 && containsMask) top = y;
                if (top != -1 && bottom == -1 && !containsMask) bottom = y;
            }

            for (int x = 0; x < mask.GetLength(1); x++)
            {
                bool containsMask = false;
                for (int y = 0; y < mask.GetLength(0); y++)
                {
                    if (mask[y, x] != 0)
                    {
                        containsMask = true;
                        break;
                    }
                }

                if (left == -1 && containsMask) left = x;
                if (left != -1 && right == -1 && !containsMask) right = x;
            }

            Point center = new Point(top + (int) ((bottom - top) / 2.0), left + (int) ((right - left) / 2.0));
            var sideLength = right - left;
            if (bottom - top > sideLength) sideLength = bottom - top;

            Console.WriteLine($"Size: {sideLength}");
            /*var bmp = new Bitmap(maskUbyte.Width, maskUbyte.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawRectangle(new Pen(Color.Red), top, left, bottom - top, right - left);
                g.DrawRectangle(new Pen(Color.Cyan), center.X - sideLength / 2, center.Y - sideLength / 2, sideLength,
                    sideLength);
            }

            var f = ddsmImage.GetDcomOriginalImage().Crop((center.X - sideLength / 2-50, center.Y - sideLength / 2-50,
                center.X + sideLength / 2+50, center.Y + sideLength / 2+50));
            f.ApplyHistogramEqualization();
            f.SaveAsPng("SpadeOliver.png");*/

            //maskUbyte.AddOverlay(bmp);
            //maskUbyte.SaveAsPng("testMask.png");
        }

        public static UshortArrayAsImage GetNormalizedSizedCrop(this UshortArrayAsImage cropped, int size)
        {
            byte[] orgPixelData = cropped.PixelData;
            int currentByteInOrgPixelData = 0;

            // lets find how many black lines we should add on each side
            int linesToAddVertical = (size - cropped.Width) / 2;
            int linesToAddHorizontal = (size - cropped.Height) / 2;

            // lets add the lines;
            byte[] pixelData = new byte[size * size * 2];

            int currentByte = 0;
            // add horizontal first, as they are the first bytes
            for (int i = 0;
                i < linesToAddHorizontal;
                i++)
            {
                for (int u = 0; u < size * 2; u++) pixelData[currentByte++] = 0;
            }

            for (int i = 0; i < cropped.Height; i++)
            {
                // for every horizontal line of img
                // add the black lines first
                for (int u = 0; u < linesToAddVertical * 2; u++) pixelData[currentByte++] = 0;
                // then add the pixeldata for the next width*2 bytes;
                for (int u = 0; u < cropped.Width * 2; u++)
                    pixelData[currentByte++] = orgPixelData[currentByteInOrgPixelData++];

                // lets add the rest of vertical lines
                for (int u = 0; u < (size - linesToAddVertical - cropped.Width) * 2; u++) pixelData[currentByte++] = 0;
            }

            return
                new UshortArrayAsImage(pixelData, size, size);
        }

        public static void Normalize(this UshortArrayAsImage ushortImg)
        {
            throw new NotImplementedException();
        }

        public static UshortArrayAsImage Edge(this UshortArrayAsImage ushortImg, int threshold)
        {
            var image = ushortImg.PixelArray;
            ushort[,] imageOverlay = image.Clone() as ushort[,];
//https://haishibai.blogspot.com/2009/09/image-processing-c-tutorial-3-edge.html
            long total = 0;
            int count = 0;
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    total += image[i, j];
                    count++;
                }
            }

            threshold = (int) (total / (double) count);
            threshold *= 1;
            Console.WriteLine($"THRESHOLD{threshold}");
            for (int i = 1; i < image.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < image.GetLength(1) - 1; j++)
                {
                    ushort cr = image[i + 1, j];
                    ushort cl = image[i - 1, j];
                    ushort cu = image[i, j - 1];
                    ushort cd = image[i, j + 1];
                    ushort cld = image[i - 1, j + 1];
                    ushort clu = image[i - 1, j - 1];
                    ushort crd = image[i + 1, j + 1];
                    ushort cru = image[i + 1, j - 1];
                    int power = GetMaxD(cr, cl, cu, cd, cld, clu, cru, crd);
                    if (power > threshold)
                    {
                        imageOverlay[i, j] = 0;
                    }
                    else
                    {
                        imageOverlay[i, j] = UInt16.MaxValue;
                    }
                }
            }

//Console.WriteLine(imageOverlay[(imageOverlay.GetLength(0)-1)/2,imageOverlay.GetLength(1)/2]);
            var x = new UshortArrayAsImage(new byte[ushortImg.Width * ushortImg.Height * 2], ushortImg.Width,
                ushortImg.Height);
            x.PixelArray = GetBiggestSector(imageOverlay, image);
//TumorDimensions(imageOverlay);
            return x;
        }

        private static int GetD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd, int[,] matrix)
        {
            return Math.Abs(matrix[0, 0] * clu + matrix[0, 1] * cu + matrix[0, 2] * cru
                            + matrix[1, 0] * cl + matrix[1, 2] * cr
                            + matrix[2, 0] * cld + matrix[2, 1] * cd + matrix[2, 2] * crd);
        }

        private static int GetMaxD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd)
        {
            int max = int.MinValue;
            for (int i = 0; i < templates.Count; i++)
            {
                int newVal = GetD(cr, cl, cu, cd, cld, clu, cru, crd, templates[i]);
                if (newVal > max)
                    max = newVal;
            }

            return max;
        }

        private static List<int[,]> templates = new List<int[,]>
        {
            new int[,] {{-3, -3, 5}, {-3, 0, 5}, {-3, -3, 5}},
            new int[,] {{-3, 5, 5}, {-3, 0, 5}, {-3, -3, -3}},
            new int[,] {{5, 5, 5}, {-3, 0, -3}, {-3, -3, -3}},
            new int[,] {{5, 5, -3}, {5, 0, -3}, {-3, -3, -3}},
            new int[,] {{5, -3, -3}, {5, 0, -3}, {5, -3, -3}},
            new int[,] {{-3, -3, -3}, {5, 0, -3}, {5, 5, -3}},
            new int[,] {{-3, -3, -3}, {-3, 0, -3}, {5, 5, 5}},
            new int[,] {{-3, -3, -3}, {-3, 0, 5}, {-3, 5, 5}}
        };

        private static double EuclideanDistance(int p1, int q1, int p2, int q2)
        {
            return Math.Sqrt(Math.Pow(q1 - p1, 2) + Math.Pow(q2 - p2, 2));
            ;
        }

        private static ushort[,] GetBiggestSector(ushort[,] imageOverlayIn, ushort[,] origin)
        {
            List<Sector> sectors = new List<Sector>();
            ushort[,] imageOverlay = imageOverlayIn.Clone() as ushort[,];
            for (int i = 0; i < imageOverlay.GetLength(0); i++)
            {
                for (int j = 0; j < imageOverlay.GetLength(1); j++)
                {
/*bool isSearched = false;
foreach (Sector x in sectors)
{
    if (x.ImgArray[i, j] == UInt16.MaxValue) isSearched = true;
}*/
                    if (imageOverlay[i, j] == UInt16.MaxValue)
                    {
// mby check here if the original image is black at this sector position, then refuse?
//if (origin[i, j] > 10000) // todo hardcode
                        Sector sectorToAdd = GetSector(imageOverlay, i, j, origin);
                        if (sectorToAdd != null)
                            sectors.Add(sectorToAdd);
                    }
                }
            }

            if (sectors.Count == 0)
                return new ushort[origin.GetLength(0),
                    origin.GetLength(1)]; // return black image if no sectors were found.
            return sectors.First(x => x.SectorSize == sectors.Max(y => y.SectorSize)).GetImgArray(imageOverlayIn);
        }

        private static Sector GetSector(ushort[,] imageOverlay, int i, int j, ushort[,] origin)
        {
            Sector result = new Sector();
            result.Point = new Point(j, i);
//result.ImgArray = imageOverlay.Clone() as ushort[,]; // this wastes memory
            result.SectorSize = GetSizeOfSector(imageOverlay, i, j, origin, out var averageColor);
            if (double.IsNaN(averageColor) || averageColor < 20000) return null; //todo hardcode
/*for (int k = 0; k < result.ImgArray.GetLength(0); k++)
{
    for (int l = 0; l < result.ImgArray.GetLength(1); l++)
    {
        if (result.ImgArray[k, l] == UInt16.MaxValue) result.ImgArray[k, l] = 0;
        if (result.ImgArray[k, l] == UInt16.MaxValue - 1) result.ImgArray[k, l] = UInt16.MaxValue;
    }
}*/
            return result;
        }

        private static int GetSizeOfSector(ushort[,] resultImgArray, int i, int j, ushort[,] origin,
            out double averageColor)
        {
// recursion was more intuitive than queue, however we got stackoverflow
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(i, j));
            int result = 0;
            int total = 0;
            while (queue.Count > 0)
            {
// run as long as points to explore
                Point p = queue.Dequeue();
                if (p.X > 0 && p.Y > 0 && p.X < resultImgArray.GetLength(0) &&
                    p.Y < resultImgArray.GetLength(1) && resultImgArray[p.X, p.Y] == UInt16.MaxValue)
                {
                    resultImgArray[p.X, p.Y] = UInt16.MaxValue - 1;
                    result++;
                    total += origin[p.X, p.Y];
                    queue.Enqueue(new Point(p.X - 1, p.Y));
                    queue.Enqueue(new Point(p.X + 1, p.Y));
                    queue.Enqueue(new Point(p.X, p.Y - 1));
                    queue.Enqueue(new Point(p.X, p.Y + 1));
                }
            }

            averageColor = total / (double) result;
            return result;
        }

        private static void TumorDimensions(ushort[,] imageOverlay)
        {
            int p1 = 0, p2 = 0, q1 = 0, q2 = 0;
            while (imageOverlay[p1, q1] != UInt16.MaxValue)
            {
                q1++;
                if (imageOverlay.GetLength(1) == q1)
                {
                    q1 = 0;
                    p1++;
                }
            }

            Console.WriteLine(imageOverlay[p1, q1]);
            Console.WriteLine(q1);
            Console.WriteLine(p1);
        }

        private class Sector
        {
            public Point Point { get; set; }

//public ushort[,] ImgArray { get; set; }
            public int SectorSize { get; set; }

            public ushort[,] GetImgArray(ushort[,] imageOverlayIn)
            {
                ushort[,] imageOverlay = imageOverlayIn.Clone() as ushort[,];
                GetSizeOfSector(imageOverlay, Point.Y, Point.X, imageOverlayIn, out var average);
                for (int k = 0; k < imageOverlay.GetLength(0); k++)
                {
                    for (int l = 0; l < imageOverlay.GetLength(1); l++)
                    {
                        if (imageOverlay[k, l] == UInt16.MaxValue) imageOverlay[k, l] = 0;
                        if (imageOverlay[k, l] == UInt16.MaxValue - 1) imageOverlay[k, l] = UInt16.MaxValue;
                    }
                }

                return imageOverlay;
            }
        }
    }
}