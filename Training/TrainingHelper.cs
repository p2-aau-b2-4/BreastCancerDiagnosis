using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord.IO;
using DimensionReduction;
using ImagePreprocessing;
using LibSVMsharp;
using LibSVMsharp.Extensions;

namespace Training
{
    public static class TrainingHelper
    {

        public class ParameterResult : IComparable
        {
            public double Accuracy { get; set; }
            public double C { get; set; }
            public double Gamma { get; set; }
            public int CompareTo(Object obj)
            {
                ParameterResult pr = obj as ParameterResult;
                if(pr == null) throw new ArgumentException("Can only compare to same type..");
                if (Math.Abs(C - pr.C) > 0.0000001)
                    return C.CompareTo(pr.C);
                return Gamma.CompareTo(pr.Gamma);
            }
        }

        public static SVMParameter FindBestHyperparameters(SVMProblem problem, SVMParameter parameter)
        {
            int nFold = int.Parse(Configuration.Get("nFold"));
            int logTo = int.Parse(Configuration.Get("logTo"));
            int logFrom = int.Parse(Configuration.Get("logFrom"));

            BlockingCollection<ParameterResult> results = new BlockingCollection<ParameterResult>();
            List<Task> tasks = new List<Task>();
            for (double cLog = logFrom; cLog <= logTo; cLog++)
            {
                double c = Math.Pow(2, cLog);
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                    {
                        SVMParameter parameterUnderTest = parameter.Clone();
                        parameterUnderTest.C = c;
                        parameterUnderTest.Gamma = Math.Pow(2, gammaLog);
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
            
            var resultList = results.ToList();
            resultList.Sort();

            ParameterResult bestParameter =
                HighestScore(resultList);

            SaveToCsv(results, "svmData.txt");
            SVMParameter returnValue = parameter.Clone();
            returnValue.C = bestParameter.C;
            returnValue.Gamma = bestParameter.Gamma;
            return returnValue;
        }

        public static void SaveToCsv(BlockingCollection<ParameterResult> results, string name)
        {
            int logTo = int.Parse(Configuration.Get("logTo"));
            int logFrom = int.Parse(Configuration.Get("logFrom"));

            // lets save the dictionary to a csv file:
            // loop through every c value and create a header, then for every gamma value, create a sideheader and insert values:
            using (StreamWriter file = new StreamWriter(name, true))
            {
                //createheader:
                file.Write($@"{"C\\G",5}"); // empty topleft corner
                for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                {
                    file.Write($"{Math.Pow(2, gammaLog),5},");
                }

                file.WriteLine();

                for (double cLog = logFrom; cLog <= logTo; cLog++)
                {
                    file.Write($"{Math.Pow(2, cLog),5},");
                    for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                    {
                        file.Write(
                            $"{Math.Round(results.Where(x => (Math.Abs(x.C - Math.Pow(2, cLog)) < 0.0001 && Math.Abs(x.Gamma - Math.Pow(2, gammaLog)) < 0.0001)).ToArray()[0].Accuracy, 2),5},");
                    }

                    file.WriteLine();
                }
            }
        }

        private static ParameterResult HighestScore(List<ParameterResult> results)
        {
            //find the highest score:
            double highestScore = results.Max(k => k.Accuracy);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return results.First(x => x.Accuracy == highestScore);
        }

        public static Pca GetPca(List<ImageWithResultModel> images)
        {
            if (File.Exists(Configuration.Get("PcaModelLocation")))
            {
                Console.WriteLine("Loaded PCA from file..");
                return Pca.LoadModelFromFile(Configuration.Get("PcaModelLocation"));
            }
            else
            {
                //train PCA:
                Pca pca = new Pca();
                Console.WriteLine("Training PCA...");
                pca.Train(images.Select(x => x.Image).ToList());
                pca.Save(Configuration.Get("PcaModelLocation"));
                Console.WriteLine("Done training and saving PCA.");
                return pca;
            }
        }
    }
}