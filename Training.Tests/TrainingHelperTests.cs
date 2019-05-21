using System;
using System.Collections.Concurrent;
using CsvHelper.Configuration;
using NUnit.Framework;
using System.Linq;
using LibSVMsharp;

namespace Training.Tests
{
    [TestFixture]
    public class TrainingHelperTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase]
        public void HighestScoreTest()
        {
            // Setup results
            BlockingCollection<TrainingHelper.ParameterResult> results =
                new BlockingCollection<TrainingHelper.ParameterResult>();
            
            int logTo = 8;
            int logFrom = -6;

            for (double cLog = logFrom; cLog <= logTo; cLog++)
            {
                for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                {
                    results.Add(new TrainingHelper.ParameterResult()
                    {
                        Accuracy = 1, C = Math.Pow(2, cLog),
                        Gamma = Math.Pow(2, gammaLog)
                    });
                }
            }
            
            //Setup func and revFunc
            Func<double, double> func = (x) => Math.Pow(x, 2);
            Func<double, double> revFunc = (x) => (Math.Log(x, 2));
            
            //Setup test SVMParameter
            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 2,
                Gamma = 1,
                Probability = true,
                WeightLabels = new[] {0, 1},
                Weights = new[] {(1 - 0.69)/0.69, 1}
            };
            
            // Expected parameterRange
            var realValue = new TrainingHelper.ParameterRange()
            {
                FromC = 0, ToC = 0, FromGamma = 0, ToGamma = 0
            };
            
            // Actual parameterRange
            var actualValue = TrainingHelper.HighestScore(results, func, revFunc, parameter);
            
            Assert.AreEqual(realValue, actualValue);

        }

        [TestCase]
        public void SaveToCSVTest()
        {

        }
    }
}