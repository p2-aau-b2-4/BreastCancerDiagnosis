using System;
using System.Collections.Generic;
using System.IO;
using Accord.IO;
using Accord.Statistics.Analysis;
using ImagePreprocessing;
using Training;
using Serializer = Accord.IO.Serializer;

namespace DimensionReduction
{
    public class newKPca
    {
        public static KernelPrincipalComponentAnalysis TrainPCA(List<ImageWithResultModel>readyImages, out double[][] data)
        {
            int imageCount = 0;
            data = new double[readyImages.Count][];
            foreach (ImageWithResultModel image in readyImages)
            {
                data[imageCount] = GetVectorFromUShortArray(image.Image.PixelArray);
                imageCount++;
            }


            KernelPrincipalComponentAnalysis pca = new KernelPrincipalComponentAnalysis();
            /*if (File.Exists("modelKPCA.bin"))
                pca = Serializer.Load<KernelPrincipalComponentAnalysis>("model.bin");
            else
            {*/
                Console.WriteLine("training kpca");
                pca.Learn(data);
            //}

            pca.Save("modelKPCA.bin");

            return pca;
        }
        public static PrincipalComponentAnalysis LoadPcaFromFile()
        {
            return Serializer.Load<PrincipalComponentAnalysis>("model.bin");
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

            return imageAsDouble;
        }
    }
}