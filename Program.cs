using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using Dicom;
using Dicom.Imaging;
using Microsoft.Win32;
using MNIST.IO;
using Accord.Statistics;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;

namespace DicomDisplayTest    
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var img = DicomFile.Open(@"e.dcm");
            UshortImageInfo imgInfo = img.GetUshortImageInfo();
            imgInfo.Render("brystDef.png");

            imgInfo.ApplyNoiseFilter(1);
            imgInfo.Render("brystNoiseFilter.png");
            
            Bitmap overlayToAdd = new Bitmap(550,550);
            for (int x = 200; x < 500; x++)
            {
                overlayToAdd.SetPixel(x,200,Color.Red);
                overlayToAdd.SetPixel(x,400,Color.Red);
            }
            for (int y = 200; y < 400; y++)
            {
                overlayToAdd.SetPixel(200,y,Color.Red);
                overlayToAdd.SetPixel(500,y,Color.Red);
            }
            imgInfo.AddOverlay(overlayToAdd);*/

            Program2.RunPCA();
            
            var data = FileReaderMNIST.LoadImagesAndLables(
                "train-labels-idx1-ubyte.gz",
                "train-images-idx3-ubyte.gz");

            var t = data.GetEnumerator();
            byte[,] image = new byte[28,28];
            
            //ImageMark[] imageArray = new ImageMark[30000];
            List<byte[,]> imageList = new List<byte[,]>();
            
            int i = 0;
            
            // Let's create an analysis with centering (covariance method)
            // but no standardization (correlation method) and whitening:
            
            
            /*while (t.MoveNext())
            {
                image = t.Current.Image;
                //imageList.Add(image);
                
                Bitmap imgBitmap = new Bitmap(28,28);
            
                for (int x = 0; x < imgBitmap.Width; x++)
                {
                    for (int y = 0; y < imgBitmap.Height; y++)
                    {
                        int greyColor = image[y,x];
                        imgBitmap.SetPixel(x, y, Color.FromArgb(greyColor, greyColor, greyColor));
                    }
                }

                var currentLabel = t.Current.Label;
                imgBitmap.Save("mnist3/" + i + "_" + currentLabel + ".png", ImageFormat.Png);
                
                t.MoveNext();
                i++;
            }*/
            
            Console.WriteLine(i);
            t.Dispose();

            //imgInfo.Render("bryst.png");
        }
    }

    public class ImageMark
    {
        private static int _size;
        private byte[,] _image = new byte[_size,_size];
        public ImageMark(byte[,] pixels, int size)
        {
            _size = size;
            _image = pixels;
        }
    }
}