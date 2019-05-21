using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using System.Linq;
using ImagePreprocessing;
using LibSVMsharp;

namespace Training.Tests
{
    [TestFixture]
    public class TrainingHelperTests
    {

        [TestCase]
        public void FindBestHyperparametersTest()
        {
            SVMProblem problem = new SVMProblem();
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 5),
                new SVMNode(2, 10),
                new SVMNode(3, 72),
                new SVMNode(4, 55),
                new SVMNode(5, 1),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 62),
                new SVMNode(2, 10),
                new SVMNode(3, 2),
                new SVMNode(4, 95),
                new SVMNode(5, 16),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 11),
                new SVMNode(2, 12),
                new SVMNode(3, 13),
                new SVMNode(4, 14),
                new SVMNode(5, 15),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 69),
                new SVMNode(2, 13),
                new SVMNode(3, 37),
                new SVMNode(4, 4),
                new SVMNode(5, 18),
            },0);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 50),
                new SVMNode(2, 100),
                new SVMNode(3, 720),
                new SVMNode(4, 550),
                new SVMNode(5, 10),
            },0);
            
            
            //Setup test SVMParameter
            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 2,
                Gamma = 1,
                Probability = true,
            };

            
            var actualParameter = TrainingHelper.FindBestHyperparameters(problem, parameter);

            Assert.AreEqual(0.015625d, actualParameter.C);
            Assert.AreEqual(0.015625d, actualParameter.Gamma);
        }
        
        [TestCase]
        public void FindBestHyperparametersGivenAnswersTest()
        {
            SVMProblem problem = new SVMProblem();
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 1),
                new SVMNode(2, 10),
                new SVMNode(3, 72),
                new SVMNode(4, 55),
                new SVMNode(5, 1),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 1),
                new SVMNode(2, 10),
                new SVMNode(3, 2),
                new SVMNode(4, 95),
                new SVMNode(5, 16),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 1),
                new SVMNode(2, 12),
                new SVMNode(3, 13),
                new SVMNode(4, 14),
                new SVMNode(5, 15),
            },1);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 0),
                new SVMNode(2, 13),
                new SVMNode(3, 37),
                new SVMNode(4, 4),
                new SVMNode(5, 18),
            },0);
            
            problem.Add(new SVMNode[]
            {
                new SVMNode(1, 0),
                new SVMNode(2, 100),
                new SVMNode(3, 720),
                new SVMNode(4, 550),
                new SVMNode(5, 10),
            },0);
            
            
            //Setup test SVMParameter
            SVMParameter parameter = new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 2,
                Gamma = 1,
                Probability = true,
            };

            
            var actualParameter = TrainingHelper.FindBestHyperparameters(problem, parameter);

            Assert.AreEqual(0.015625d, actualParameter.C);
            Assert.AreEqual(0.125d, actualParameter.Gamma);
        }

        [TestCase]
        public void SaveToCsvTest()
        {
            
            // Setup results
            BlockingCollection<TrainingHelper.ParameterResult> results =
                new BlockingCollection<TrainingHelper.ParameterResult>();
            
            int logTo = int.Parse(Configuration.Get("logTo"));
            int logFrom = int.Parse(Configuration.Get("logFrom"));

            for (double cLog = logFrom; cLog <= logTo; cLog++)
            {
                for (double gammaLog = logFrom; gammaLog <= logTo; gammaLog++)
                {
                    results.Add(new TrainingHelper.ParameterResult()
                    {
                        Accuracy = 1, C = Math.Pow(2,cLog),
                        Gamma = Math.Pow(2,gammaLog)
                    });
                }
                
            }
            
            File.Delete(@"svmDataActual.txt");
            
            TrainingHelper.SaveToCsv(results, "svmDataActual.txt");

            FileInfo expectedFile = new FileInfo(@"svmDataExpected.txt");
            FileInfo actualFile = new FileInfo(@"svmDataActual.txt");
            
            
            FileAssert.AreEqual(expectedFile, actualFile);
        }
        
        
        [TestCase]
        public void SaveToCsvOutOfBoundsTest()
        {
            
            // Setup results
            BlockingCollection<TrainingHelper.ParameterResult> results =
                new BlockingCollection<TrainingHelper.ParameterResult>();
            
            
            File.Delete(@"svmDataActual.txt");

            Assert.Throws<IndexOutOfRangeException>(() => TrainingHelper.SaveToCsv(results, "svmDataActual.txt"));
        }
    }
}