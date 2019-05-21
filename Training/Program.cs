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

            Pca pca = TrainingHelper.GetPca(imagesToTrainOn);

            Console.WriteLine($"{pca.ComponentVectors.Length}x{pca.ComponentVectors[0].Length}");
            int[] componentsArr = new[] {50, 100, 200, pca.Eigenvalues.Length};
            foreach (int components in componentsArr)
            {
                TrainAndTestSvm(pca, imagesToTrainOn, imagesToTestOn, components);
            }
        }

        private static void TrainAndTestSvm(Pca pca, List<ImageWithResultModel> imagesToTrainOn,
            List<ImageWithResultModel> imagesToTestOn, int components)
        {
            Console.WriteLine($"Training with #{components}...");
            SVMProblem trainingSet;
            SVMProblem testSet;

            if (Configuration.Get("ShouldCreateSVMSetsWithPCA").Equals("1"))
            {
                trainingSet = GetProblemFromImageModelResultList(imagesToTrainOn, pca,components);
                testSet = GetProblemFromImageModelResultList(imagesToTestOn, pca,components);

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

            // lets silence libsvm..
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
                    $"PCACOMPONENTS={components} C={parameter.C}, GAMMA={parameter.Gamma} testAccuracy={testAccuracy}, sensitivity={sensitivity}, specificity={specificity}");
            }


            double[] results = testSet.PredictProbability(model, out var probabilities);

            int errors = 0;
            for (int i = 0; i < probabilities.Count; i++)
            {
                String x = "";
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (results[i] != testSet.Y[i])
                {
                    x = " FUCKED UP!!! ";
                    errors++;
                }

                Console.WriteLine(
                    $"{results[i]} | {probabilities[i][0]} | {probabilities[i][1]} | {testSet.Y[i]} | {x}");
            }
        }

        private static void CreateAndSaveImages()
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

        private static List<ImageWithResultModel> TransformDdsmImageList(List<DdsmImage> images)
        {
            List<ImageWithResultModel> result = new List<ImageWithResultModel>();
            List<DdsmImage> imagesCc = images.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Cc)).ToList();
            foreach (DdsmImage image in imagesCc)
            {
                //if (image.Pathology == DdsmImage.Pathologies.BenignWithoutCallback) continue; todo
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
            Pca pca,int components)
        {
//            if (!int.TryParse(Configuration.Get("componentsToUse"), out int components))
//            {
//                components = pca.Eigenvalues.Length;
//            }

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