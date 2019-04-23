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
        List<DdsmImage> DDSMImages;

        string filePath;
        string testValueResult;

        [SetUp]
        public void Setup()
        {

            DDSMImages =
            DdsmImage.GetAllImagesFromCsvFile(@"D:\Bryster\mass_case_description_test_set.csv");

        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest1()
        {
            
            filePath = DDSMImages.ElementAt(0).DcomFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC\10-04-2016-DDSM-30104\1-full mammogram images-14172\000000.dcm";

            Assert.AreEqual(testValueResult, filePath);

        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest2()
        {
            filePath = DDSMImages.ElementAt(1).DcomFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_MLO\10-04-2016-DDSM-54392\1-full mammogram images-35518\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest3()
        {
            filePath = DDSMImages.ElementAt(2).DcomFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00017_LEFT_CC\10-04-2016-DDSM-12683\1-full mammogram images-82967\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetCroppedImages()
        {
            filePath = DDSMImages.ElementAt(0).DcomMaskFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC_1\10-04-2016-DDSM-09887\1-cropped images-26184\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetMaskImages()
        {
            filePath = DDSMImages.ElementAt(0).DcomCroppedFilePath;
            testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC_1\10-04-2016-DDSM-09887\1-cropped images-26184\000001.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }



    }
}

