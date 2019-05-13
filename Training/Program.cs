using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
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
//            List<DdsmImage> ddsmImagesTrain =
//                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
//            ddsmImagesTrain = ddsmImagesTrain
//                .Where(image => image.Pathology != DdsmImage.Pathologies.BenignWithoutCallback).ToList();
//            TransformAndSaveImagesWithResultModels(ddsmImagesTrain,"readyImageModelTrain.bin");
//            List<DdsmImage> ddsmImagesTest =
//               DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_test_set.csv"); //todo use configurationManager
//            ddsmImagesTest = ddsmImagesTest
//                .Where(image => image.Pathology != DdsmImage.Pathologies.BenignWithoutCallback).ToList();
//            TransformAndSaveImagesWithResultModels(ddsmImagesTest,"readyImageModelTest.bin");

            TrainPcaAndSvm();
        }

        private static void TrainPcaAndSvm()
        {
//            List<ImageWithResultModel> imagesToTrainOn =
//                Serializer.Load<List<ImageWithResultModel>>("readyImageModelTrain.bin");
//            List<ImageWithResultModel> imagesToTestOn =
//                Serializer.Load<List<ImageWithResultModel>>("readyImageModelTest.bin");
//
//            // create the pca MODEL:
//            List<ImageWithResultModel> pcaTrainSet = imagesToTrainOn.DeepClone();
//            pcaTrainSet.AddRange(imagesToTestOn);
//
//            PrincipalComponentAnalysis pca = newPca.TrainPCA(pcaTrainSet, out var data);
//
//            // with this, then let us create the two svm problems.
//            SVMProblem trainingSet = GetProblemFromImageModelResultList(imagesToTrainOn, pca, 200);
//            SVMProblem testSet = GetProblemFromImageModelResultList(imagesToTestOn, pca, 200);
//
//            testSet.Save("test.txt");
//            trainingSet.Save("train.txt");

            SVMProblem trainingSet = SVMProblemHelper.Load("train.txt");
            SVMProblem testSet = SVMProblemHelper.Load("test.txt");


            trainingSet = trainingSet.Normalize(SVMNormType.L2);
            testSet = testSet.Normalize(SVMNormType.L2);

            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 1,
                Gamma = 1,
                Probability = true,
            };

            Console.WriteLine("training svm");
            Console.WriteLine(System.AppContext.BaseDirectory);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            ///
            ///
            ///
            /// //////////////////////////////////////////////////////////////////////////////////////////////////////
            double[] crossValidationResults; // output labels
            int nFold = 5;
            trainingSet.CrossValidation(parameter, nFold, out crossValidationResults);

            // Evaluate the cross validation result
            // If it is not good enough, select the parameter set again
            double crossValidationAccuracy = trainingSet.EvaluateClassificationProblem(crossValidationResults);

            // Train the model, If your parameter set gives good result on cross validation
            SVMModel model = trainingSet.Train(parameter);

            // Save the model
            SVM.SaveModel(model, @"phishing_model.txt");

            // Predict the instances in the test set
            double[] testResults = testSet.Predict(model);


            // Evaluate the test results
            double testAccuracy = testSet.EvaluateClassificationProblem(testResults, model.Labels, out var confusionMatrix);

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

//            List<double[]> derpshit = new List<double[]>();
//            double[] derp = testSet.PredictProbability(model, out derpshit);
//            for (int i = 0; i < derpshit.Count; i++)
//            {
//                String x = "";
//                if (derp[i] != testSet.Y[i]) x = " FUCKED UP!!! ";
//                Console.WriteLine(
//                    $"Resultat|Sandsynlighed for sand|Sandsynlighed for falsk: {derp[i]} | {derpshit[i][0]} | {derpshit[i][1]} | {testSet.Y[i]} | {x}");
//            }
        }

        private static void TransformAndSaveImagesWithResultModels(List<DdsmImage> images, string saveLoc)
        {
            ConcurrentBag<ImageWithResultModel> readyImages = new ConcurrentBag<ImageWithResultModel>();

            //readyImages.Add(Normalization.GetNormalizedImage(DDSMImages[0].DcomOriginalImage, Normalization.GetTumourPositionFromMask(DDSMImages[0].DcomMaskImage),100));
            List<DdsmImage> imagesCc = images.Where(x => (x.ImageView == DdsmImage.ImageViewEnum.Cc)).ToList();
            //new List<UShortArrayAsImage>(readyImages).Save("readyImages.bin");
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
                    svmNodes[i] = new SVMNode(i + 1, pcaDownsizedComponents[i]);
                }

                problem.Add(svmNodes, image.Result);
            }

            return problem;
        }
    }
}