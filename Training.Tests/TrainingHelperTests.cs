using System;
using System.Collections.Concurrent;
using System.IO;
using CsvHelper.Configuration;
using NUnit.Framework;

namespace Training.Tests
{
    [TestFixture]
    public class TrainingHelperTests
    {

        [TestCase]
        public void SaveToCSVTest()
        {
            
            BlockingCollection<TrainingHelper.ParameterResult> results =
                new BlockingCollection<TrainingHelper.ParameterResult>();

            int nFold = 5;
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
            
            TrainingHelper.SaveToCSV(results, "svmDataActual.txt");

            FileInfo expectedFile = new FileInfo(@"svmDataExpected.txt");
            FileInfo actualFile = new FileInfo(@"svmDataActual.txt");
            
            
            FileAssert.AreEqual(expectedFile, actualFile);
        }
    }
}