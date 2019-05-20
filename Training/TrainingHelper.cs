using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using DimensionReduction;
using ImagePreprocessing;
using LibSVMsharp;
using LibSVMsharp.Extensions;

namespace Training
{
    public static class TrainingHelper
    {
        private class ParameterRange
        {
            public double FromC { get; set; }
            public double ToC { get; set; }
            public double FromGamma { get; set; }
            public double ToGamma { get; set; }
        }

        private class ParameterResult
        {
            public double Accuracy { get; set; }
            public double C { get; set; }
            public double Gamma { get; set; }
        }

        /// <summary>
        /// This function shall find the best hyperparameters (C and Gamma)
        /// for a specific training problem via cross-validation,
        /// as suggested in https://www.csie.ntu.edu.tw/~cjlin/papers/guide/guide.pdf
        /// </summary>
        /// <param name="problem">The training problem</param>
        /// <param name="parameter">Initial parameters</param>
        /// <returns>A new parameter set, only C and gamma will have been optimized</returns>
        public static SVMParameter FindBestHyperparameters(SVMProblem problem, SVMParameter parameter)
        {
//            ParameterRange parameterCoarseRange =
//                FindOptimalRange(new ParameterRange() {FromC = Math.Pow(0.001, FromGamma = 0.01, ToC = 100, ToGamma = 100},
//                    (x) => (Math.Pow(x, 2)), (x) => (Math.Log(x,2)), problem, parameter);

            ParameterRange parameterFineRange = FindOptimalRangeUsingLog(new ParameterRange(), (x) => Math.Pow(x, 2),
                (x) => (Math.Log(x,2)), problem, parameter, true);
            parameter.C = parameterFineRange.FromC;
            parameter.Gamma = parameterFineRange.FromGamma;
            return parameter;
        }

        private static ParameterRange FindOptimalRange(ParameterRange parameterRange, Func<double, double> func,
            Func<double, double> revFunc,
            SVMProblem problem, SVMParameter parameter, bool returnFromValuesAsAnswer = false)
        {
            int nFold = int.Parse(Configuration.Get("nFold"));

            BlockingCollection<ParameterResult> results = new BlockingCollection<ParameterResult>();
            List<Task> tasks = new List<Task>();
            for (double cIn = parameterRange.FromC; cIn <= parameterRange.ToC; cIn = func(cIn))
            {
                double c = cIn.DeepClone();
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                    {
                        SVMParameter parameterUnderTest = parameter.Clone();
                        parameterUnderTest.C = c;
                        parameterUnderTest.Gamma = gamma;
                        problem.CrossValidation(parameterUnderTest, nFold, out var crossValidationResults);
                        double crossValidationAccuracy = problem.EvaluateClassificationProblem(crossValidationResults);

                        results.Add(new ParameterResult()
                        {
                            Accuracy = crossValidationAccuracy, C = parameterUnderTest.C,
                            Gamma = parameterUnderTest.Gamma
                        });
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            
            //find the highest score:
            double highestScore = 0;
            SVMParameter bestParameter = parameter.Clone();
            foreach (ParameterResult result in results.ToList())
            {
                if (result.Accuracy > highestScore)
                {
                    highestScore = result.Accuracy;
                    bestParameter.C = result.C;
                    bestParameter.Gamma = result.Gamma;
                }
            }

            if (bestParameter == null) throw new Exception("Something went really wrong, no parameters was found.");

            ParameterRange fineParameterRange = new ParameterRange();
            if (!returnFromValuesAsAnswer)
            {
                fineParameterRange.ToC = func(bestParameter.C);
                fineParameterRange.FromC = revFunc(bestParameter.C);
                fineParameterRange.ToGamma = func(bestParameter.Gamma);
                fineParameterRange.FromGamma = revFunc(bestParameter.Gamma);
            }
            else
            {
                fineParameterRange.FromC = bestParameter.C;
                fineParameterRange.FromGamma = bestParameter.Gamma;
            }

            // lets save the dictionary to a csv file:
            // loop through every c value and create a header, then for every gamma value, create a sideheader and insert values:
            using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
            {
                //createheader:
                file.Write($@"{"C\\G",5}"); // empty topleft corner
                for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                {
                    file.Write($"{gamma,5},");
                }

                file.WriteLine();

                for (double c = parameterRange.FromC; c <= parameterRange.ToC; c = func(c))
                {
                    file.Write($"{c,5},");
                    for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                    {
                        file.Write($"{Math.Round(results.Where(x => (Math.Abs(x.C - c) < 0.0001 && Math.Abs(x.Gamma - gamma) < 0.0001)).ToArray()[0].Accuracy,2),5},");
                    }

                    file.WriteLine();
                }
            }

            return fineParameterRange;
        }
        private static ParameterRange FindOptimalRangeUsingLog(ParameterRange parameterRange, Func<double, double> func,
            Func<double, double> revFunc,
            SVMProblem problem, SVMParameter parameter, bool returnFromValuesAsAnswer = false)
        {
            int nFold = int.Parse(Configuration.Get("nFold"));
            int logTo = 8;
            int logFrom = -6;
            BlockingCollection<ParameterResult> results = new BlockingCollection<ParameterResult>();
            List<Task> tasks = new List<Task>();
            for (double cLog = logFrom; cLog <= logTo; cLog++)
            {
                double c = Math.Pow(2,cLog);
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                    {
                        SVMParameter parameterUnderTest = parameter.Clone();
                        parameterUnderTest.C = c;
                        parameterUnderTest.Gamma = Math.Pow(2,gammaLog);
                        problem.CrossValidation(parameterUnderTest, nFold, out var crossValidationResults);
                        double crossValidationAccuracy = problem.EvaluateClassificationProblem(crossValidationResults);

                        results.Add(new ParameterResult()
                        {
                            Accuracy = crossValidationAccuracy, C = parameterUnderTest.C,
                            Gamma = parameterUnderTest.Gamma
                        });
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            
            //find the highest score:
            double highestScore = 0;
            SVMParameter bestParameter = parameter.Clone();
            foreach (ParameterResult result in results.ToList())
            {
                if (result.Accuracy > highestScore)
                {
                    highestScore = result.Accuracy;
                    bestParameter.C = result.C;
                    bestParameter.Gamma = result.Gamma;
                }
            }

            if (bestParameter == null) throw new Exception("Something went really wrong, no parameters was found.");

            ParameterRange fineParameterRange = new ParameterRange();
            if (!returnFromValuesAsAnswer)
            {
                fineParameterRange.ToC = func(bestParameter.C);
                fineParameterRange.FromC = revFunc(bestParameter.C);
                fineParameterRange.ToGamma = func(bestParameter.Gamma);
                fineParameterRange.FromGamma = revFunc(bestParameter.Gamma);
            }
            else
            {
                fineParameterRange.FromC = bestParameter.C;
                fineParameterRange.FromGamma = bestParameter.Gamma;
            }

            // lets save the dictionary to a csv file:
            // loop through every c value and create a header, then for every gamma value, create a sideheader and insert values:
            using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
            {
                //createheader:
                file.Write($@"{"C\\G",5}"); // empty topleft corner
                for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                {
                    file.Write($"{Math.Pow(2,gammaLog),5},");
                }

                file.WriteLine();

                for (double cLog = logFrom; cLog <= logTo; cLog++)
                {
                    file.Write($"{Math.Pow(2,cLog),5},");
                    for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                    {
                        file.Write($"{Math.Round(results.Where(x => (Math.Abs(x.C - Math.Pow(2,cLog)) < 0.0001 && Math.Abs(x.Gamma - Math.Pow(2,gammaLog)) < 0.0001)).ToArray()[0].Accuracy,2),5},");
                    }

                    file.WriteLine();
                }
            }

            return fineParameterRange;
        }
        
        

        public static PCA GetPca(List<ImageWithResultModel> images)
        {
            if (File.Exists(Configuration.Get("PcaModelLocation")))
            {
                Console.WriteLine("Loaded PCA from file..");
                return PCA.LoadModelFromFile(Configuration.Get("PcaModelLocation"));
            }
            else
            {
                //train PCA:
                PCA pca = new PCA();
                Console.WriteLine("Training PCA...");
                List<UShortArrayAsImage> imagesUShort = new List<UShortArrayAsImage>();
                int i = 0;
                foreach (var image in images)
                    if (i++ % 1 == 0)
                        imagesUShort.Add(image.Image);
                pca.Train(imagesUShort);
                pca.Save(Configuration.Get("PcaModelLocation"));
                Console.WriteLine("Done training and saving PCA.");
                return pca;
            }
        }
    }
}