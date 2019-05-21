using System.Collections.Concurrent;
using NUnit.Framework;

namespace Training.Tests
{
    [TestFixture]
    public class TrainingHelperTests
    {

        [TestCase]
        public void SaveToCSVTest()
        {
            BlockingCollection<TrainingHelper.ParameterResult> results = new BlockingCollection<TrainingHelper.ParameterResult>();
            
            
            
            TrainingHelper.SaveToCSV(results);
            Assert.Pass();
        }
        
    }
}