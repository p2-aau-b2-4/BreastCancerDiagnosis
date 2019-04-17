using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class DDSMImageTest
    {
        [TestCase]
        public void GetAllImagesFromCsvFile()
        {
            List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"C:\Bryster\mass_case_description_test_set.csv");

            string filePath = DDSMImages.First().DcomFilePath;
            string testValueResult = @"C:\Bryster\Mass-Test_P_00016_LEFT_CC\10-04-2016-DDSM-30104\1-full mammogram images-14172\000000.dcm";

            Assert.AreEqual(testValueResult, filePath);
        }



    }
}

