using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
using Accord.Math;
using DimensionReduction;
using ImagePreprocessing;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using LibSVMsharp.Helpers;
using Serializer = Accord.IO.Serializer;

namespace Training
{
    public static class ProblemHandler
    {
        public class SvmResult
        {
            public double C { get; set; }
            public double Gamma { get; set; }
            public double TestAccuracy { get; set; }
            public double Sensitivity { get; set; }
            public double Specificity { get; set; }
        }
        
        public static SvmResult TrainAndTestSvm(SVMProblem trainingSet, SVMProblem testSet)
        {

            // find the ratio of malignant:benign cases:
            double mbTrainRatio = trainingSet.Y.Where(x => x == 0).ToArray().Length*1F/trainingSet.Y.Count;
            Console.WriteLine($"MB TRAIN RATIO: {mbTrainRatio}");
            double mbTestRatio = testSet.Y.Where(x => x == 0).ToArray().Length * 1F / testSet.Y.Count;
            Console.WriteLine($"MB TEST RATIO: {mbTestRatio}");

            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = double.Parse(Configuration.Get("C")),
                Gamma = double.Parse(Configuration.Get("Gamma")),
                Probability = true,
                WeightLabels = new[] {0, 1},
                Weights = new[] {(1 - mbTrainRatio)/mbTrainRatio, 1}
            };

            parameter = TrainingHelper.FindBestHyperparameters(trainingSet, parameter);
            Console.WriteLine($"Found best parameters: c={parameter.C},gamma={parameter.Gamma}");

            SVMModel model = trainingSet.Train(parameter);
            SVM.SaveModel(model, Configuration.Get("ModelLocation"));

            // The following evaluation has code from:
            // https://csharp.hotexamples.com/examples/LibSVMsharp/SVMParameter/-/php-svmparameter-class-examples.html
            
            // Predict the instances in the test set
            double[] testResults = testSet.Predict(model);


            // Evaluate the test results
            double testAccuracy =
                testSet.EvaluateClassificationProblem(testResults, model.Labels, out var confusionMatrix);

            // Print the resutls
            Console.WriteLine("\nTest accuracy: " + testAccuracy);
            Console.WriteLine("\nConfusion matrix:\n");


            // Print formatted confusion matrix
            
            Console.Write($"{"",6}");
            for (int i = 0; i < model.Labels.Length; i++)
                Console.Write($"{"(" + model.Labels[i] + ")",5}");
            Console.WriteLine();
            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
            {
                Console.Write($"{"(" + model.Labels[i] + ")",5}");
                for (int j = 0; j < confusionMatrix.GetLength(1); j++)
                    Console.Write($"{confusionMatrix[i, j],5}");
                Console.WriteLine();
            }

            double sensitivity = confusionMatrix[0, 0] * 1.0 /
                                 (confusionMatrix[0, 1] + confusionMatrix[0, 0]);
            double specificity = confusionMatrix[1, 1] * 1.0 /
                                 (confusionMatrix[1, 1] + confusionMatrix[1, 0]);
            


            double[] results = testSet.PredictProbability(model, out var probabilities);
            for (int i = 0; i < probabilities.Count; i++)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                String x = results[i] != testSet.Y[i] ? "MISPREDICTION" :"";
                Console.WriteLine($"{results[i]} | {probabilities[i][0]} | {probabilities[i][1]} | {testSet.Y[i]} | {x}");
            }

            return new SvmResult()
            {
                C = parameter.C, Gamma = parameter.Gamma, TestAccuracy = testAccuracy, Sensitivity = sensitivity,
                Specificity = specificity
            };
        }

        public static void CreateAndSaveImages()
        {
            List<DdsmImage> trainingSetDdsm = new List<DdsmImage>();
            List<DdsmImage> testSetDdsm = new List<DdsmImage>();

            (string, List<DdsmImage>)[] paths =
            {
                ("massTrainingSetCsvPath", trainingSetDdsm), ("massTestSetCsvPath", testSetDdsm),
                ("calcTrainingSetCsvPath", trainingSetDdsm), ("calcTestSetCsvPath", testSetDdsm)
            };


            foreach ((string, List<DdsmImage>) path in paths)
            {
                path.Item2.AddRange(DdsmImage.GetAllImagesFromCsvFile(Configuration.Get(path.Item1)));
            }

            Task<List<ImageWithResultModel>> trainTask = Task.Factory.StartNew(
                () => TransformDdsmImageList(trainingSetDdsm),
                TaskCreationOptions.LongRunning);
            Task<List<ImageWithResultModel>> testTask = Task.Factory.StartNew(
                () => TransformDdsmImageList(testSetDdsm),
                TaskCreationOptions.LongRunning);
            Task.WaitAll(trainTask, testTask);
            List<ImageWithResultModel> trainingSet = trainTask.Result;
            List<ImageWithResultModel> testSet = testTask.Result;

            trainingSet.Save(Configuration.Get("TrainReadyImage"));
            testSet.Save(Configuration.Get("TestReadyImage"));
        }

        public static List<ImageWithResultModel> TransformDdsmImageList(List<DdsmImage> images)
        {
            List<ImageWithResultModel> result = new List<ImageWithResultModel>();
            List<DdsmImage> imagesCc = images.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Mlo)).ToList();
            foreach (DdsmImage image in imagesCc)
            {
                Console.WriteLine($"{result.Count * 100 / imagesCc.Count}% done");
                var imageResult = new ImageWithResultModel
                {
                    Result = image.Pathology == DdsmImage.Pathologies.Malignant ? 1 : 0,
                    Image = Contrast.ApplyHistogramEqualization(
                        Normalization.GetNormalizedImage(image.DcomOriginalImage,
                            Normalization.GetTumourPositionFromMask(image.DcomMaskImage),
                            int.Parse(Configuration.Get("sizeImageToAnalyze"))))
                };
                result.Add(imageResult);
            }

            return result;
        }


        public static SVMProblem GetProblemFromImageModelResultList(List<ImageWithResultModel> images,
            Pca pca,int components)
        {
            SVMProblem problem = new SVMProblem();
            foreach (ImageWithResultModel image in images)
            {
                double[] pcaComponents = pca.GetComponentsFromImage(image.Image, components);
                bool valid = false;
                foreach (double x in pcaComponents)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (x != 0) valid = true;
                }

                if (!valid) continue;
                SVMNode[] svmNodes = new SVMNode[pcaComponents.Length];
                for (int i = 0; i < pcaComponents.Length; i++)
                {
                    // index is i+1, because libsvm has index 0 reserved.
                    svmNodes[i] = new SVMNode(i + 1, pcaComponents[i]);
                }

                problem.Add(svmNodes, image.Result);
            }

            Console.WriteLine("Problem is done");

            return problem;
        }
    }
}
