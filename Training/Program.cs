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

            Pca pca = TrainingHelper.GetPca(imagesToTrainOn);

            Console.WriteLine($"{pca.ComponentVectors.Length}x{pca.ComponentVectors[0].Length}");
            int[] componentsArr = new[] {50, 100, 200, pca.Eigenvalues.Length};
            foreach (int components in componentsArr)
            {
                Console.WriteLine($"Training with #{components}...");
                SVMProblem trainingSet;
                SVMProblem testSet;
                if (Configuration.Get("ShouldCreateSVMSetsWithPCA").Equals("1"))
                {
                    trainingSet = ProblemHandler.GetProblemFromImageModelResultList(imagesToTrainOn, pca, components);
                    testSet = ProblemHandler.GetProblemFromImageModelResultList(imagesToTestOn, pca, components);

                    testSet.Save(Configuration.Get("TestSetLocation"));
                    trainingSet.Save(Configuration.Get("TrainSetLocation"));
                }
                else
                {
                    trainingSet = SVMProblemHelper.Load(Configuration.Get("TrainSetLocation"));
                    testSet = SVMProblemHelper.Load(Configuration.Get("TestSetLocation"));
                }


                trainingSet = trainingSet.Normalize(SVMNormType.L2);
                testSet = testSet.Normalize(SVMNormType.L2);

                testSet.Save("testNormalized.txt");
                trainingSet.Save("trainNormalized.txt");


                ProblemHandler.SvmResult result = ProblemHandler.TrainAndTestSvm(trainingSet, testSet);
                using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
                {
                    file.WriteLine(
                        $"PCACOMPONENTS={components}, C={result.C}, GAMMA={result.Gamma} testAccuracy={result.TestAccuracy}, sensitivity={result.Sensitivity}, specificity={result.Specificity}");
                }
            }
        }
    }
}