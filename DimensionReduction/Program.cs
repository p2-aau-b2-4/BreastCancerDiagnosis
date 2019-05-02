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
using DimensionReduction;
using ImagePreprocessing;
using Microsoft.Win32;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;


namespace DicomDisplayTest    
{
    class Program
    {
        static void Main(string[] args) {
            PCA pca = new PCA();
            //List<DdsmImage> DDSMImages =
            //    DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            //Console.WriteLine($"Found {DDSMImages.Count}");
            
            
            List<UShortArrayAsImage> list = new List<UShortArrayAsImage>();
            UShortArrayAsImage image1 = Normalization.GetNormalizedImage(DicomFile.Open("000000.dcm").GetUshortImageInfo(), new Rectangle(0,0,100,100),100 );
            
            //list.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),50));
            //list.Add(Normalization.GetNormalizedImage(DDSMImages[1].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[1].DcomMaskImage),50));
            for (int i = 0; i < 10000; i++)
            {
                
                list.Add(image1);
            }

            Console.WriteLine("list is done");
            pca.Train(list);
        }
    }
}
