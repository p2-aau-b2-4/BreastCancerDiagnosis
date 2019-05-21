using System.Collections.Concurrent;
using CsvHelper.Configuration;
using NUnit.Framework;

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
                        Accuracy = 1, C = 2,
                        Gamma = 3
                    });
                }

                TrainingHelper.SaveToCSV(results);
                Assert.Pass();
            }

        }
    }
}