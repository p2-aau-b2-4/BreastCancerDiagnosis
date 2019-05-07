using System.Collections.Generic;
using System.IO;
using Accord.Statistics.Analysis;
using ImagePreprocessing;
using Serializer = Accord.IO.Serializer;

namespace DimensionReduction
{
    public class newPca
    {
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