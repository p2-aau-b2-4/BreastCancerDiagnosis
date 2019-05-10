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
using Accord.Statistics.Analysis;


namespace DicomDisplayTest
{
    class Program
    {
        static void Main(string[] args)
        {
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
            imagesToTrainOn = Serializer.Load<List<UShortArrayAsImage>>("readyImagesTrain.bin");
            //imagesToTrainOn.AddRange(Serializer.Load<List<UShortArrayAsImage>>("readyImagesTest.bin"));
            pca2.Train(imagesToTrainOn);
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