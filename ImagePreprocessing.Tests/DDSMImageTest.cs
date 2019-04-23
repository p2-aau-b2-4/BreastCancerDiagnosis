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
                DdsmImage.GetAllImagesFromCsvFile(@"D:\Bryster\mass_case_description_test_set.csv");

            string filePath = DDSMImages.ElementAt(0).DcomFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC\10-04-2016-DDSM-30104\1-full mammogram images-14172\000000.dcm";

            Assert.AreEqual(testValueResult, filePath);

            filePath = DDSMImages.ElementAt(1).DcomFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_MLO\10-04-2016-DDSM-54392\1-full mammogram images-35518\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);


            filePath = DDSMImages.ElementAt(2).DcomFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00017_LEFT_CC\10-04-2016-DDSM-12683\1-full mammogram images-82967\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }



    }
}

