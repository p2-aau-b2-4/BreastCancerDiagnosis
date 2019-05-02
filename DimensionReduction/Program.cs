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
            List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");
            
            
            List<UShortArrayAsImage> list = new List<UShortArrayAsImage>();
            //UShortArrayAsImage image1 = Normalization.DicomFile.Open("000000.dcm").GetUshortImageInfo();
            
            list.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),50));
            list.Add(Normalization.GetNormalizedImage(DDSMImages[1].DcomOriginalImage,Normalization.GetTumourPositionFromMask(DDSMImages[1].DcomMaskImage),50));
            
            //list.Add(image1);
            /*list.Add(image1);*/
            
            /*SparseMatrix sparseMatrix = SparseMatrix.OfArray(arr);

            sparseMatrix = pca.MeanSubtraction(sparseMatrix);
            sparseMatrix = pca.CovarianceMatrix(sparseMatrix);
            pca.SolveEigenValues(sparseMatrix);
            pca.SolveEigenVectors(sparseMatrix);*/
            
            //PCA2 pca2 = new PCA2();
            
            
            //pca2.Train(bitmaps);

            pca.Train(list);
            
            
            

            /*sM5 = pca.MeanSubtraction(sM5);
            sM5 = pca.CovarianceMatrix(sM5);

            for (int xIndex = 0; xIndex < 4; xIndex++)
            {
                for (int yIndex = 0; yIndex < 4; yIndex++)
                {
                    Console.Write(sM5[xIndex, yIndex]);
                }

                Console.WriteLine();
            }

            pca.SolveEigenValues(sM5);
            pca.SolveEigenValues(sM5);*/
            
            // Example #3: Write only some strings in an array to a file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            // NOTE: do not use FileStream for text files because it writes bytes, but StreamWriter
            // encodes the output as text.
            
            
        }
    }
}
