using System;
using System.Collections.Generic;
using System.IO;
using Accord.IO;
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
            ParameterRange parameterCoarseRange =
                FindOptimalRange(new ParameterRange() {FromC = 0.01, FromGamma = 0.01, ToC = 1000, ToGamma = 1000},
                    (x) => (x * 10), (x) => (x / 10F), problem, parameter);

            ParameterRange parameterFineRange = FindOptimalRange(parameterCoarseRange, (x) => (x * 2),
                (x) => (x / 2F), problem, parameter);
            parameter.C = parameterFineRange.ToC-(parameterFineRange.ToC - parameterFineRange.FromC);
            parameter.Gamma = parameterFineRange.ToGamma-(parameterFineRange.ToGamma - parameterFineRange.FromGamma);
            return parameter;
        }

        private static ParameterRange FindOptimalRange(ParameterRange parameterRange, Func<double, double> func,
            Func<double, double> revFunc,
            SVMProblem problem, SVMParameter parameter)
        {
            int nFold = int.Parse(Configuration.Get("nFold"));

            Dictionary<SVMParameter, double> results = new Dictionary<SVMParameter, double>();

            for (double c = parameterRange.FromC; c <= parameterRange.ToC; c = func(c))
            {
                for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                {
                    SVMParameter parameterUnderTest = parameter.Clone();
                    parameterUnderTest.C = c;
                    parameterUnderTest.Gamma = gamma;
                    problem.CrossValidation(parameterUnderTest, nFold, out var crossValidationResults);
                    double crossValidationAccuracy = problem.EvaluateClassificationProblem(crossValidationResults);

                    results.Add(parameterUnderTest,crossValidationAccuracy);
                }
            }

            //find the highest score:
            double highestScore = 0;
            SVMParameter bestParameter = null;
            foreach (KeyValuePair<SVMParameter, double> pair in results)
            {
                if (pair.Value > highestScore)
                {
                    highestScore = pair.Value;
                    bestParameter = pair.Key;
                }
            }

            if (bestParameter == null) throw new Exception("Something went really wrong, no parameters was found.");

            parameterRange.ToC = func(bestParameter.C);
            parameterRange.FromC = revFunc(bestParameter.C);
            parameterRange.ToGamma = func(bestParameter.C);
            parameterRange.FromGamma = revFunc(bestParameter.C);
            
            // lets save the dictionary to a csv file:
            // loop through every c value and create a header, then for every gamma value, create a sideheader and insert values:
            using (StreamWriter file = new StreamWriter(@"svmData.txt", true))
            {
                //createheader:
                file.Write("cvalue,"); // empty topleft corner
                for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                {
                    file.Write($"{gamma},");
                }
                file.WriteLine();

                for (double c = parameterRange.FromC; c <= parameterRange.ToC; c = func(c))
                {
                    file.Write($"{c},");
                    for (double gamma = parameterRange.FromGamma; gamma <= parameterRange.ToGamma; gamma = func(gamma))
                    {
                        SVMParameter parameterUnderTest = parameter.Clone();
                        parameterUnderTest.C = c;
                        parameterUnderTest.Gamma = gamma;
                        file.Write($"{results.GetValueOrDefault(parameterUnderTest)},");
                    }
                    file.WriteLine();
                }
            }
            return parameterRange;
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
                foreach(var image in images) if(i++ % 1 == 0) imagesUShort.Add(image.Image);
                pca.Train(imagesUShort);
                pca.Save(Configuration.Get("PcaModelLocation"));
                Console.WriteLine("Done training and saving PCA.");
                return pca;
            }
        }
    }
}