using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
using DimensionReduction;
using ImagePreprocessing;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using LibSVMsharp.Helpers;
using Serializer = Accord.IO.Serializer;

namespace Training
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            if (Configuration.Get("ShouldCreateImages").Equals("1"))
            {
                ProblemHandler.CreateAndSaveImages();
            }

            List<ImageWithResultModel> imagesToTrainOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TrainReadyImage"));
            List<ImageWithResultModel> imagesToTestOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TestReadyImage"));

            PCA pca = TrainingHelper.GetPca(imagesToTrainOn);

            Console.WriteLine($"{pca.ComponentVectors.Length}x{pca.ComponentVectors[0].Length}");
            int[] componentsArr = new[] {50, 100, 200, pca.Eigenvalues.Length};
            foreach (int components in componentsArr)
            {
                ProblemHandler.TrainAndTestSvm(pca, imagesToTrainOn, imagesToTestOn, components);
            }
        }
    }
}