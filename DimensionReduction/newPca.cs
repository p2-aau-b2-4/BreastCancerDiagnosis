using System;
using System.Collections.Generic;
using System.IO;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Analysis;
using ImagePreprocessing;
using Training;
using Serializer = Accord.IO.Serializer;

namespace DimensionReduction
{
    public class newPca
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readyImages"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PrincipalComponentAnalysis TrainPCA(List<ImageWithResultModel>readyImages, out double[][] data)
        {
            //reduce data:
            List<ImageWithResultModel> toTrainOn = new List<ImageWithResultModel>();
            for (int i = 0; i < readyImages.Count; i++)
            {
                if(i % 2 == 0) toTrainOn.Add(readyImages[i]);
            }

            readyImages = toTrainOn;
            
            int imageCount = 0;
            data = new double[readyImages.Count][];
            foreach (ImageWithResultModel image in readyImages)
            {
                data[imageCount] = GetVectorFromUShortArray(image.Image.PixelArray);
                imageCount++;
            }

            //data = data.Transpose();

            Console.WriteLine("done creating data");


            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
            if (File.Exists("model.bin"))
                pca = Serializer.Load<PrincipalComponentAnalysis>("model.bin");
            else
            {
                Console.WriteLine("training pca");
                
                
                
                
                
                pca.Learn(data);
            }

            Console.WriteLine(pca.Eigenvalues.Length);
            pca.Save("model.bin");

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