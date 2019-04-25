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
            double[,] arr = new double[4,4] {
              {2, 0, 0, 3},
              {0, 0, 0, 0},
              {0, 5, 7, 0},
              {0, 4, 0, 0}
            };
            LinearAlgebra.Matrix m = new LinearAlgebra.Matrix(arr);
            SparseMatrix ms = SparseMatrix.OfArray(arr);

            Console.WriteLine(m.ToString());
            
            double[,] matrix1 = new double[,]
            {
                {2.5,2.4},
                {0.5,0.7},
                {2.2,2.9},
                {1.9,2.2},
                {3.1,3.0},
                {2.3,2.7},
                {2.0,1.6},
                {1.0,1.1},
                {1.5,1.6},
                {1.1,0.9}
            };
            
            double[,] matrix2 = new double[,]
            {
                {0,0,0,0},
                {0,0,0,0},
                {0,0,0,0},
                {0,0,0,0}
            };
            
            SparseMatrix sM1 = SparseMatrix.OfArray(matrix1);
            SparseMatrix sM4 = SparseMatrix.OfArray(arr);
            SparseMatrix sM5 = SparseMatrix.OfArray(matrix2);
            
            PCA pca = new PCA();
            //List<DdsmImage> DDSMImages =
            //    DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            //Console.WriteLine($"Found {DDSMImages.Count}");
            List<UshortArrayAsImage> list = new List<UshortArrayAsImage>();
            UshortArrayAsImage image1 = DicomFile.Open("/home/yogi/Documents/Uni/2_semester/P2/data/Calc-Training_P_01847_LEFT_CC/08-07-2016-DDSM-15869/1-full mammogram images-70974/000000.dcm").GetUshortImageInfo();
            //UshortArrayAsImage image2 = DicomFile.Open("/home/yogi/Documents/Uni/2_semester/P2/data/Calc-Training_P_01847_LEFT_CC/08-07-2016-DDSM-15869/1-full mammogram images-70974/000000.dcm").GetUshortImageInfo();
            //UshortArrayAsImage image3 = DicomFile.Open("/home/yogi/Documents/Uni/2_semester/P2/data/Calc-Training_P_01847_LEFT_CC/08-07-2016-DDSM-15869/1-full mammogram images-70974/000000.dcm").GetUshortImageInfo();
            //UshortArrayAsImage image4 = DicomFile.Open("/home/yogi/Documents/Uni/2_semester/P2/data/Calc-Training_P_01847_LEFT_CC/08-07-2016-DDSM-15869/1-full mammogram images-70974/000000.dcm").GetUshortImageInfo();
            
            
            list.Add(image1);
            list.Add(image1);
            list.Add(image1);
            /*list.Add(image1);*/
            
            /*SparseMatrix sparseMatrix = SparseMatrix.OfArray(arr);

            sparseMatrix = pca.MeanSubtraction(sparseMatrix);
            sparseMatrix = pca.CovarianceMatrix(sparseMatrix);
            pca.SolveEigenValues(sparseMatrix);
            pca.SolveEigenVectors(sparseMatrix);*/
            
            //PCA2 pca2 = new PCA2();
            
            PCA3 pca3 = new PCA3();
            
            List<Bitmap> bitmaps = new List<Bitmap>();

            int ii = 0;
            foreach (string img in Directory.GetFiles("mnist2"))
            {
                /*if(ii > 99)
                    break;
                ii++;*/
                bitmaps.Add(new Bitmap(img));
            }
            
            pca3.Train(bitmaps);
            
            //pca2.Train(bitmaps);

            //pca.Train(list);

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
