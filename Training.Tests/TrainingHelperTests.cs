using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using CsvHelper.Configuration;
using NUnit.Framework;
using System.Linq;
using LibSVMsharp;

namespace Training.Tests
{
    [TestFixture]
    public class TrainingHelperTests
    {

        [TestCase]
        public void GetPcaLoadTest()
        {
            
        }

        [TestCase]
        public void FindBestHyperparametersTest()
        {
            SVMProblem problem = new SVMProblem();
            SVMNode[] svmNodes = new SVMNode[2];
            svmNodes[0] = new SVMNode(1, 5);
            svmNodes[1] = new SVMNode(2, 7);
            
            
            SVMNode[] svmNodes1 = new SVMNode[2];
            svmNodes1[0] = new SVMNode(3, 10);
            svmNodes1[1] = new SVMNode(4, 15);

            
            SVMNode[] svmNodes2 = new SVMNode[2];
            svmNodes2[0] = new SVMNode(5, 0);
            svmNodes2[1] = new SVMNode(6, 3);
            
            problem.Add(svmNodes, 1);
            problem.Add(svmNodes1, 0);
            problem.Add(svmNodes2, 0);
            
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

            
            // Actual parameter
            List<double> gammaValues = new List<double> {0.015625, 0.5, 0.25};

            List<double> cValues = new List<double>
            {
                2,
                1,
                0.0625,
                0.5,
                0.03125,
                0.25,
                0.125,
                0.015625
            };


            if (cValues.Contains(actualParameter.C) && gammaValues.Contains(actualParameter.Gamma))
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestCase]
        public void SaveToCSVTest()
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
    }
}