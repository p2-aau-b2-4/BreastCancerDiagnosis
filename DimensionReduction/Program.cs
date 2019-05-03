using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Accord.IO;
using Dicom;
using Dicom.Imaging;
using DimensionReduction;
using ImagePreprocessing;
using Microsoft.Win32;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using Serializer = Accord.IO.Serializer;


namespace DicomDisplayTest    
{
    class Program
    {
        static void Main(string[] args) {
            PCA pca = new PCA();
            
            /*List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_test_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");
            BlockingCollection<UShortArrayAsImage> readyImages = new BlockingCollection<UShortArrayAsImage>();
            
            Console.WriteLine(readyImages.Count);
            readyImages.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage, Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),100));
            
            new List<UShortArrayAsImage>(readyImages).Save("readyImages.bin");

            Parallel.ForEach(DDSMImages, image =>
            {
                Console.WriteLine(readyImages.Count);
                readyImages.Add(Normalization.GetNormalizedImage(image.DcomOriginalImage, Normalization.GetTumourPositionFromMask(image.DcomMaskImage),100));
            });
            new List<UShortArrayAsImage>(readyImages).Save("readyImages.bin");*/
            
            List<UShortArrayAsImage> readyImages = Serializer.Load<List<UShortArrayAsImage>>("readyImages.bin");
            readyImages[0].SaveAsPng("testhn.png");
            
            
            
            /*List<UShortArrayAsImage> list = new List<UShortArrayAsImage>();
            UShortArrayAsImage image1 = Normalization.GetNormalizedImage(DicomFile.Open("000000.dcm").GetUshortImageInfo(), new Rectangle(0,0,100,100),100 );
            
            //list.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),50));
            //list.Add(Normalization.GetNormalizedImage(DDSMImages[1].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[1].DcomMaskImage),50));
            for (int i = 0; i < 10; i++)
            {
                list.Add(image1);
            }*/

            Console.WriteLine("list is done");
            pca.Train(readyImages);
            pca.model.SaveModelToFile("fuckJulis.xml");
        }
    }
}
