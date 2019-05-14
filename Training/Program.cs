using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
using Accord.Math;
using Accord.Statistics.Analysis;
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
        static void Main(string[] args)
        {
            //TrainWithDifferentParameters();
           //PrintHighestCrossValidation();
//            Console.WriteLine(Environment.CurrentDirectory);
//            if (Configuration.Get("ShouldCreateImages").Equals("1"))
//            {
//                List<DdsmImage> trainingSetDdsm = new List<DdsmImage>();
//                List<DdsmImage> testSetDdsm = new List<DdsmImage>();
//
//                (string, List<DdsmImage>)[] paths =
//                {
//                    ("massTrainingSetCsvPath", trainingSetDdsm), ("massTestSetCsvPath", testSetDdsm),
//                    ("calcTrainingSetCsvPath", trainingSetDdsm), ("calcTestSetCsvPath", testSetDdsm)
//                };
//
//
//                foreach ((string, List<DdsmImage>) path in paths)
//                {
//                    path.Item2.AddRange(DdsmImage.GetAllImagesFromCsvFile(Configuration.Get(path.Item1)));
//                }
//
//                Task<List<ImageWithResultModel>> trainTask = Task.Factory.StartNew(
//                    () => TransformDdsmImageList(trainingSetDdsm),
//                    TaskCreationOptions.LongRunning);
//                Task<List<ImageWithResultModel>> testTask = Task.Factory.StartNew(
//                    () => TransformDdsmImageList(testSetDdsm),
//                    TaskCreationOptions.LongRunning);
//                Task.WaitAll(new Task[] {trainTask, testTask});
//                List<ImageWithResultModel> trainingSet = trainTask.Result;
//                List<ImageWithResultModel> testSet = testTask.Result;
//
//                trainingSet.Save(Configuration.Get("TrainReadyImage"));
//                testSet.Save(Configuration.Get("TestReadyImage"));
//            }
            TrainPcaAndSvm();
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

        private static void TrainPcaAndSvm()
        {
            List<ImageWithResultModel> imagesToTrainOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TrainReadyImage"));
            List<ImageWithResultModel> imagesToTestOn =
                Serializer.Load<List<ImageWithResultModel>>(Configuration.Get("TestReadyImage"));
            Console.WriteLine(
                $"{imagesToTrainOn.Where(x => x.Result == 1).ToList().Count * 1.0 / imagesToTrainOn.Where(x => x.Result == 0).ToList().Count}");

            SVMProblem trainingSet;
            SVMProblem testSet;

            if (Configuration.Get("ShouldCreateSVMSetsWithPCA").Equals("1"))
            {
                // create the pca MODEL:
                List<ImageWithResultModel> pcaTrainSet = imagesToTrainOn.DeepClone();

                PrincipalComponentAnalysis pca = newPca.TrainPCA(pcaTrainSet, out var data);

                // with this, then let us create the two svm problems.
                trainingSet = GetProblemFromImageModelResultList(imagesToTrainOn, pca,
                    int.Parse(Configuration.Get("componentsToUse")));
                testSet = GetProblemFromImageModelResultList(imagesToTestOn, pca,
                    int.Parse(Configuration.Get("componentsToUse")));

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

            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = double.Parse(Configuration.Get("C")),
                Gamma = double.Parse(Configuration.Get("Gamma")),
                Probability = true,
                WeightLabels = new int[] {0, 1},
                Weights = new double[] {1-0.69949494949495, 0.69949494949495}
            };

            Console.WriteLine("training svm");

            double[] crossValidationResults; // output labels
            int nFold = int.Parse(Configuration.Get("nFold"));
            trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

            // Evaluate the cross validation result
            // If it is not good enough, select the parameter set again
            double crossValidationAccuracy = trainingSet.EvaluateClassificationProblem(crossValidationResults);

            // Train the model, If your parameter set gives good result on cross validation
            SVMModel model = trainingSet.Train(parameter);

            // Save the model
            SVM.SaveModel(model, Configuration.Get("ModelLocation"));

            // Predict the instances in the test set
            double[] testResults = testSet.Predict(model);


            // Evaluate the test results
            double testAccuracy =
                testSet.EvaluateClassificationProblem(testResults, model.Labels, out var confusionMatrix);

            // Print the resutls
            Console.WriteLine("\n\nCross validation accuracy: " + crossValidationAccuracy);
            Console.WriteLine("\nTest accuracy: " + testAccuracy);
            Console.WriteLine("\nConfusion matrix:\n");


            // Print formatted confusion matrix
            Console.Write(String.Format("{0,6}", ""));
            for (int i = 0; i < model.Labels.Length; i++)
                Console.Write(String.Format("{0,5}", "(" + model.Labels[i] + ")"));
            Console.WriteLine();
            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
            {
                Console.Write(String.Format("{0,5}", "(" + model.Labels[i] + ")"));
                for (int j = 0; j < confusionMatrix.GetLength(1); j++)
                    Console.Write(String.Format("{0,5}", confusionMatrix[i, j]));
                Console.WriteLine();
            }

            double sensitivitity = confusionMatrix[0, 0] * 1.0 /
                                   (confusionMatrix[0, 1] + confusionMatrix[0, 0]);
            double specificity = confusionMatrix[1, 1] * 1.0 /
                                 (confusionMatrix[1, 1] + confusionMatrix[1, 0]);
            using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
            {
                file.WriteLine(
                    $"C={1.4}, GAMMA={0.3} testAccuracy={testAccuracy}, sensitivity={sensitivitity}, specificity={specificity}");
            }


            List<double[]> derpshit = new List<double[]>();
            double[] derp = testSet.PredictProbability(model, out derpshit);


            for (int i = 0; i < derpshit.Count; i++)
            {
                String x = "";
                if (derp[i] != testSet.Y[i]) x = " FUCKED UP!!! ";
                Console.WriteLine(
                    $"Resultat|Sandsynlighed for sand|Sandsynlighed for falsk: {derp[i]} | {derpshit[i][0]} | {derpshit[i][1]} | {testSet.Y[i]} | {x}");
            }
        }

        private static void TransformAndSaveImagesWithResultModels(List<DdsmImage> images, string saveLoc)
        {
            ConcurrentBag<ImageWithResultModel> readyImages = new ConcurrentBag<ImageWithResultModel>();
            List<DdsmImage> imagesCc = images.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Cc)).ToList();
            Console.WriteLine($"CC: {imagesCc.Count}");
            Parallel.ForEach(imagesCc, image =>
            {
                Console.WriteLine($"{readyImages.Count * 100 / imagesCc.Count}% done");
                var imageResult = new ImageWithResultModel();
                imageResult.Result = image.Pathology == DdsmImage.Pathologies.Malignant ? 1 : 0;

                imageResult.Image = Contrast.ApplyHistogramEqualization(Normalization.GetNormalizedImage(
                    image.DcomOriginalImage,
                    Normalization.GetTumourPositionFromMask(image.DcomMaskImage), 500));
                readyImages.Add(imageResult);
            });

            new List<ImageWithResultModel>(readyImages).Save(saveLoc);
        }

        private static SVMProblem GetProblemFromImageModelResultList(List<ImageWithResultModel> images,
            PrincipalComponentAnalysis pca, int components)
        {
            //GetPcaData:
            //reduce data:
            int imageCount = 0;
            var data = new double[images.Count][];
            foreach (ImageWithResultModel image in images)
            {
                data[imageCount] = newPca.GetVectorFromUShortArray(image.Image.PixelArray);
                imageCount++;
            }

            Console.WriteLine(pca.Eigenvalues.Length);
            Console.WriteLine(pca.Components.Count);
            //data = data.Transpose();

            //double[][] pcaComponents2d = pca.Transform(data);
            
            
            
            
            SVMProblem problem = new SVMProblem();
            foreach (ImageWithResultModel image in images)
            {
                double[] vector = newPca.GetVectorFromUShortArray(image.Image.PixelArray);
                double[] pcaComponents = pca.Transform(vector);
                double[] pcaDownsizedComponents;
                if (components >= pcaComponents.Length) pcaDownsizedComponents = pcaComponents;
                else
                {
                    pcaDownsizedComponents = new double[components];
                    for (int i = 0; i < pcaDownsizedComponents.Length; i++)
                        pcaDownsizedComponents[i] = pcaComponents[i];
                }

                SVMNode[] svmNodes = new SVMNode[pcaDownsizedComponents.Length];
                for (int i = 0; i < pcaDownsizedComponents.Length; i++)
                {
                    // index is i+1, because libsvm has index 0 reserved.
                    svmNodes[i] = new SVMNode(i + 1, pcaDownsizedComponents[i]);
                }

                problem.Add(svmNodes, image.Result);
            }

            return problem;
        }

        private static void TrainWithDifferentParameters()
        {
            List<Task> tasks = new List<Task>();
            for (double cInit = 0.01; cInit <= 10000; cInit *= 10)
            {
                double c = cInit.DeepClone();
                tasks.Add(new Task(() =>
                {
                    for (double gamma = 0.01; gamma <= 10000; gamma *= 10)
                    {
                        SVMProblem trainingSet = SVMProblemHelper.Load(Configuration.Get("TrainSetLocation"));
                        SVMProblem testSet = SVMProblemHelper.Load(Configuration.Get("TestSetLocation"));


                        trainingSet = trainingSet.Normalize(SVMNormType.L2);
                        testSet = testSet.Normalize(SVMNormType.L2);

                        SVMParameter parameter = new SVMParameter
                        {
                            Type = SVMType.C_SVC,
                            Kernel = SVMKernelType.RBF,
                            C = c,
                            Gamma = gamma,
                            Probability = false,
                            //WeightLabels = new int[] {0, 1},
                            //Weights = new double[] {1-0.638190954773869, 0.638190954773869}
                        };

                        Console.WriteLine($"training svm with c={c},gamma={gamma}");

                        double[] crossValidationResults; // output labels
                        int nFold = int.Parse(Configuration.Get("nFold"));
                        trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

                        // Evaluate the cross validation result
                        // If it is not good enough, select the parameter set again
                        double crossValidationAccuracy =
                            trainingSet.EvaluateClassificationProblem(crossValidationResults);

                        // Train the model, If your parameter set gives good result on cross validation
                        SVMModel model = trainingSet.Train(parameter);

                        // Save the model
                        SVM.SaveModel(model, Configuration.Get("ModelLocation"));

                        // Predict the instances in the test set
                        double[] testResults = testSet.Predict(model);


                        // Evaluate the test results
                        double testAccuracy =
                            testSet.EvaluateClassificationProblem(testResults, model.Labels, out var confusionMatrix);

                        // Print the resutls
                        Console.WriteLine("\n\nCross validation accuracy: " + crossValidationAccuracy);
                        Console.WriteLine("\nTest accuracy: " + testAccuracy);
                        double sensitivitity = confusionMatrix[0, 0] * 1.0 /
                                               (confusionMatrix[0, 1] + confusionMatrix[0, 0]);
                        double specificity = confusionMatrix[1, 1] * 1.0 /
                                             (confusionMatrix[1, 1] + confusionMatrix[1, 0]);
                        using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
                        {
                            file.WriteLine(
                                $"C={c}, GAMMA={gamma} testAccuracy={testAccuracy}, sensitivity={sensitivitity}, specificity={specificity}");
                        }
                    }
                }));
            }

            foreach (Task x in tasks) x.Start();

            Task.WaitAll(tasks.ToArray());
        }

        private static void PrintHighestCrossValidation()
        {
            SVMProblem trainingSet = SVMProblemHelper.Load(Configuration.Get("TrainSetLocation"));
            SVMProblem testSet = SVMProblemHelper.Load(Configuration.Get("TestSetLocation"));


            trainingSet = trainingSet.Normalize(SVMNormType.L2);
            testSet = testSet.Normalize(SVMNormType.L2);
            for (double c = 0.01; c <= 10000; c *= 10)
            {
                {
                    for (double gamma = 0.01; gamma <= 10000; gamma *= 10)
                    {
                        SVMParameter parameter = new SVMParameter
                        {
                            Type = SVMType.C_SVC,
                            Kernel = SVMKernelType.RBF,
                            C = c,
                            Gamma = gamma,
                            Probability = false,
                            WeightLabels = new int[] {0, 1},
                            Weights = new double[] {0.638190954773869, 1}
                        };


                        double[] crossValidationResults; // output labels
                        int nFold = int.Parse(Configuration.Get("nFold"));
                        trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

                        // Evaluate the cross validation result
                        // If it is not good enough, select the parameter set again
                        double crossValidationAccuracy =
                            trainingSet.EvaluateClassificationProblem(crossValidationResults);
                        Console.WriteLine($"training svm with c={c},gamma={gamma}, cross={crossValidationAccuracy}");
                    }
                }
            }
        }
    }
}