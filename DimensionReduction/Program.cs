﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using Dicom;
using Dicom.Imaging;
using DimensionReduction;
using ImagePreprocessing;
using Serializer = Accord.IO.Serializer;
//using Extreme.Mathematics;
using Extreme.Statistics;
//using Extreme.Statistics.Multivariate;
using MathNet.Numerics;
using Matrix = Extreme.Mathematics.Matrix;
using Vector = Accord.Math.Vector;
using Accord.IO;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using MathNet.Numerics.LinearAlgebra.Double;


namespace DicomDisplayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //this is used for only blackbox..
            PCA pca2 = new PCA();
            //DicomFile.Open("mask.dcm").GetUByteImageInfo().SaveAsPng("test.png");/*
            //PCA pca = new PCA();

            /*List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"train/mass_case_description_test_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");
            List<UShortArrayAsImage> readyImages = new List<UShortArrayAsImage>();

            Console.WriteLine(readyImages.Count);
            //readyImages.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage, Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),100));
            List<DdsmImage> imagesCC = DDSMImages.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Cc)).ToList();
            //new List<UShortArrayAsImage>(readyImages).Save("readyImages.bin");
            Console.WriteLine($"CC: {imagesCC.Count}");
            //Parallel.ForEach(imagesCC, image =>
            foreach (DdsmImage image in imagesCC)
            {
                Console.WriteLine(readyImages.Count);
                readyImages.Add(Normalization.GetNormalizedImage(image.DcomOriginalImage,
                    Normalization.GetTumourPositionFromMask(image.DcomMaskImage), 100));
            }

            ;
            //readyImages.Save("readyImagesTest.bin");*/

            List<UShortArrayAsImage> imagesToTrainOn = new List<UShortArrayAsImage>();
            List<UShortArrayAsImage> imagesToTrainOnn = new List<UShortArrayAsImage>();
            imagesToTrainOn = Serializer.Load<List<UShortArrayAsImage>>("readyImagesTrain.bin");
            //imagesToTrainOn.AddRange(Serializer.Load<List<UShortArrayAsImage>>("readyImagesTest.bin"));
            imagesToTrainOn.GetRange(0, 10);

            foreach (var VAR in imagesToTrainOn)
            {
                imagesToTrainOnn.Add(Normalization.ResizeImage(VAR, 5));
            }
            
            pca2.Train(imagesToTrainOnn.GetRange(0,10));
            
            double[] mt = pca2.GetComponentsFromImage(imagesToTrainOn[21],9);
            ;
            double[,] matrix = new double[10,2] 
            {
                {1.507, 0.988},
                {2.107, -9.312},
                {1.407, 1.798},
                {1.397, 2.098},
                {-9.563, 1.988},
                {0.797, 0.888},
                {2.607, 0.488},
                {-0.493, 1.588},
                {0.627, -0.412},
                {-0.393, -0.112}
            };
            
            
            PrincipalComponentAnalysis pca4 = new PrincipalComponentAnalysis(PrincipalComponentMethod.CovarianceMatrix);

            var images = imagesToTrainOnn;
            double[,] allImages = new double[images.Count, images[0].Width * images[0].Height];
            int i = 0;
            foreach (var image in images)
            {
                double[,] tempI = new double[image.Width, image.Height];
                Array.Copy(image.PixelArray, tempI, image.PixelArray.Length);
                double[] dImage = new double[image.Width * image.Height];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        //dImage[y * image.Width + x] = image.GetPixel(x,y).R;
                        dImage[y * image.Width + x] = tempI[x, y];
                    }
                }

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        allImages[i, y * image.Width + x] = dImage[y * image.Width + x];
                    }
                }

                i++;
                Console.WriteLine($"Copied image #{i}");
            }


            pca4.Learn(allImages.Covariance().ToJagged());
            
            PCA pca3 = new PCA();
            SparseMatrix res = SparseMatrix.OfArray(matrix);// pca3.MeanSubtraction(SparseMatrix.OfArray(matrix));
            pca3.Train(res.ToArray());
            Console.WriteLine(res);

            //PrincipalComponentAnalysis pca = TrainPCA(imagesToTrainOn, out var data);


            //Console.WriteLine(pca.GetNumberOfComponents(1f));
            /* ushort[,] imageAsUshortarrayPre = new ushort[100, 100];
 
             for (int y = 0; y < 100; y++)
             {
                 for (int x = 0; x < 100; x++)
                 {
                     //if(resultVector[y*100+x] < UInt16.MinValue) Console.WriteLine(resultVector[y*100+x]);
                     double result = data[0][y * 100 + x];
                     if (result < 0) result = 0;
                     imageAsUshortarrayPre[y, x] = (ushort) Math.Round(result);
                     //Console.WriteLine($"{result} to {imageAsUshortarrayPre[y,x]}");
                 }
             }
 
             new UShortArrayAsImage(imageAsUshortarrayPre).SaveAsPng("foerPca.png");*/
            //double[] imageAsVector = GetVectorFromUShortArray(imagesToTrainOn[0].PixelArray);

//            Console.WriteLine($"Data: {data.Length},{data[0].Length}");
            /*       double[] imageToTransform =
                       GetVectorFromUShortArray(DicomFile.Open("000000.dcm").GetUshortImageInfo().PixelArray);
                   double[] imagesAsComponents = pca.Transform(imageToTransform);
       
                   Console.WriteLine($"Components: {imagesAsComponents.Length}");
       
       
                   double[][] imagesAsComponents2d = {imagesAsComponents};
                   double[][] imageRevertedComponents = pca.Revert(imagesAsComponents2d);
       
                   Console.WriteLine($"ImagesAsVectors:{imageRevertedComponents.Length},{imageRevertedComponents[0].Length}");
                   Console.WriteLine($"ComponentsVector: {pca.ComponentVectors.Length}x{pca.ComponentVectors[0].Length}");
       
                   //imageRevertedComponents[0][0] = new Random().Next();
                   
                   double[] resultVector = pca.ComponentVectors.Transpose().Dot(imagesAsComponents);
                   resultVector = resultVector.Add(pca.Means);
       
                   ushort[,] imageAsUshortarray = new ushort[100, 100];
       
                   for (int y = 0; y < 100; y++)
                   {
                       for (int x = 0; x < 100; x++)
                       {
                           //if(resultVector[y*100+x] < UInt16.MinValue) Console.WriteLine(resultVector[y*100+x]);
                           double result = resultVector[y * 100 + x];
                           imageAsUshortarray[y, x] = (ushort) Math.Round(result);
                           //Console.WriteLine($"{result} to {imageAsUshortarray[y,x]}");
                       }
                   }
       
       
                   new UShortArrayAsImage(imageAsUshortarray).SaveAsPng("efterPca.png");
                   pca.Save("model.bin");
                   while (true)
                   {
                       double number;
                       string read = "";
                       do
                       {
                           read = Console.ReadLine();
                       } while (!double.TryParse(read, out number));
       
                       Console.WriteLine($"Setting number to {number}");
                       Console.WriteLine($"Original value was {imagesAsComponents[1]}");
                       imagesAsComponents[1] = number;
                       resultVector = pca.ComponentVectors.Transpose().Dot(imagesAsComponents);
                       resultVector = resultVector.Add(pca.Means);
       
                       imageAsUshortarray = new ushort[100, 100];
       
                       for (int y = 0; y < 100; y++)
                       {
                           for (int x = 0; x < 100; x++)
                           {
                               //if(resultVector[y*100+x] < UInt16.MinValue) Console.WriteLine(resultVector[y*100+x]);
                               double result = resultVector[y * 100 + x];
                               imageAsUshortarray[y, x] = (ushort) Math.Round(result);
                               //Console.WriteLine($"{result} to {imageAsUshortarray[y,x]}");
                           }
                       }
                       new UShortArrayAsImage(imageAsUshortarray).SaveAsPng("efterPca.png");
                   }
                   
               }
       
               public static PrincipalComponentAnalysis TrainPCA(List<UShortArrayAsImage> readyImages, out double[][] data)
               {
                   int imageCount = 0;
                   data = new double[readyImages.Count][];
                   foreach (UShortArrayAsImage image in readyImages)
                   {
                       data[imageCount] = GetVectorFromUShortArray(image.PixelArray);
                       imageCount++;
                       //if (imageCount == 100) break;
                   }
       
                   //data  = data.Transpose();
       
                   //Console.WriteLine($"list is done {data.Length},{data[0].Length}");
       
       
                   PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
                   if (File.Exists("model.bin"))
                       pca = Serializer.Load<PrincipalComponentAnalysis>("model.bin");
                   else
                   {
                       pca.Learn(data);
                   }
       
                   return pca;
               }
       
               public static double[] GetVectorFromUShortArray(ushort[,] pixelArray)
               {
                   double[] imageAsDouble = new double[pixelArray.Length];
                   for (int y = 0; y < pixelArray.GetLength(0); y++)
                   {
                       for (int x = 0; x < pixelArray.GetLength(1); x++)
                       {
                           imageAsDouble[y * pixelArray.GetLength(0) + x] = pixelArray[y, x];
                       }
                   }
       
                   return Vector.Create(imageAsDouble);*/
        }
    }
}