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
                CreateAndSaveImages();
            }
            
            List<ImageWithResultModel> imagesToTrainOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TrainReadyImage"));
            List<ImageWithResultModel> imagesToTestOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TestReadyImage"));

            PCA pca = TrainingHelper.GetPca(imagesToTrainOn);
            
            Console.WriteLine($"{pca.ComponentVectors.Length}x{pca.ComponentVectors[0].Length}");

            TrainAndTestSvm(pca, imagesToTrainOn, imagesToTestOn);
        }

        private static void TrainAndTestSvm(PCA pca, List<ImageWithResultModel> imagesToTrainOn, List<ImageWithResultModel> imagesToTestOn)
        {
            SVMProblem trainingSet;
            SVMProblem testSet;

            if (Configuration.Get("ShouldCreateSVMSetsWithPCA").Equals("1"))
            {
                trainingSet = GetProblemFromImageModelResultList(imagesToTrainOn, pca);
                testSet = GetProblemFromImageModelResultList(imagesToTestOn, pca);

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
            
            // find the ratio of malignant:benign cases:
            double mbTrainRatio = imagesToTrainOn.Where(x => (x.Result == 1)).ToArray().Length * 1F /
                                  imagesToTrainOn.Where(x => x.Result == 0).ToArray().Length;
            Console.WriteLine($"MB TRAIN RATIO: {mbTrainRatio}");
            double mbTestRatio = imagesToTestOn.Where(x => (x.Result == 1)).ToArray().Length * 1F /
                                  imagesToTestOn.Where(x => x.Result == 0).ToArray().Length;
            Console.WriteLine($"MB TEST RATIO: {mbTestRatio}");

            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = double.Parse(Configuration.Get("C")),
                Gamma = double.Parse(Configuration.Get("Gamma")),
                Probability = false,
                //WeightLabels = new[] {0, 1},
                //Weights = new[] {1-mbTrainRatio, mbTrainRatio}
            };

            parameter = TrainingHelper.FindBestHyperparameters(trainingSet, parameter);
            Console.WriteLine($"Found best parameters: c={parameter.C},gamma={parameter.Gamma}");
            parameter.Probability = true;

            SVMModel model = trainingSet.Train(parameter);
            SVM.SaveModel(model, Configuration.Get("ModelLocation"));

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
            using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
            {
                file.WriteLine(
                    $"C={parameter.C}, GAMMA={parameter.Gamma} testAccuracy={testAccuracy}, sensitivity={sensitivity}, specificity={specificity}");
            }


            double[] results = testSet.PredictProbability(model, out var probabilities);


            for (int i = 0; i < probabilities.Count; i++)
            {
                String x = "";
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (results[i] != testSet.Y[i]) x = " FUCKED UP!!! ";
                Console.WriteLine(
                    $"{results[i]} | {probabilities[i][0]} | {probabilities[i][1]} | {testSet.Y[i]} | {x}");
            }
        }
/*
        private static SVMProblem OwnNormalize(SVMProblem problem)
        {
            // (x-u)/s where u is the mean, and s is the standard deviation
            List<SVMNode[]> data = problem.X;
            
            // loop through every dimension:
            // calculate mean:
            double[] means = new double[data[0].Length];
            for (int i = 0; i < data[0].Length; i++)
            {
                int count = 0;
                double amount = 0;
                foreach (SVMNode[] nodeArr in data)
                {
                    amount += nodeArr[i].Value;
                    count++;
                }
                means[i] = amount / count;
            }
            
            // calculate standard deviation:
            double[] standardDeviations = new double[data[0].Length];
            for (int i = 0; i < data[0].Length; i++)
            {
                double u = 0;
                foreach (SVMNode[] nodeArr in data)
                {
                    u += Math.Pow(nodeArr[i].Value-means[i],2);
                }
                
                standardDeviations[i] = Math.Sqrt((1F / (data[0].Length)) * u);
            }
            
            // now we can calculate each problem:
            SVMProblem result = new SVMProblem();
            int nodeIndex = 0;
            foreach (var node in data)
            {
                SVMNode[] nodeToAdd = new SVMNode[node.Length];
                for (int i = 0; i < node.Length; i++)
                {   
                    nodeToAdd[i] = new SVMNode(node[i].Index,(node[i].Value-means[i])/standardDeviations[i]);
                }
                result.Add(nodeToAdd, problem.Y[nodeIndex]);

                nodeIndex++;
            }
            return result;
        }
*/

        private static void CreateAndSaveImages()
        {
            List<DdsmImage> trainingSetDdsm = new List<DdsmImage>();
            List<DdsmImage> testSetDdsm = new List<DdsmImage>();

            (string, List<DdsmImage>)[] paths =
            {
                ("massTrainingSetCsvPath", trainingSetDdsm), ("massTestSetCsvPath", testSetDdsm),
                //("calcTrainingSetCsvPath", trainingSetDdsm), ("calcTestSetCsvPath", testSetDdsm)
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

        private static List<ImageWithResultModel> TransformDdsmImageList(List<DdsmImage> images)
        {
            List<ImageWithResultModel> result = new List<ImageWithResultModel>();
            List<DdsmImage> imagesCc = images.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Cc)).ToList();
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



        private static SVMProblem GetProblemFromImageModelResultList(List<ImageWithResultModel> images,
            PCA pca)
        {


            int components = int.Parse(Configuration.Get("componentsToUse"));

            SVMProblem problem = new SVMProblem();
            foreach (ImageWithResultModel image in images)
            {
                double[] pcaComponents = pca.GetComponentsFromImage(image.Image,components);
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