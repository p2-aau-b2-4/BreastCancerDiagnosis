using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class DDSMImageTest
    {
        List<DdsmImage> DDSMImages;


        [SetUp]
        public void Setup()
        {
            DDSMImages =
            DdsmImage.GetAllImagesFromCsvFile(@"D:\Bryster\mass_case_description_test_set.csv");

        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest1()
        {

            string filePath = DDSMImages.ElementAt(0).DcomFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC\10-04-2016-DDSM-30104\1-full mammogram images-14172\000000.dcm";

            Assert.AreEqual(testValueResult, filePath);

        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest2()
        {
            string filePath = DDSMImages.ElementAt(1).DcomFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_MLO\10-04-2016-DDSM-54392\1-full mammogram images-35518\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetAllImagesFromCsvFileTest3()
        {
            string filePath = DDSMImages.ElementAt(2).DcomFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00017_LEFT_CC\10-04-2016-DDSM-12683\1-full mammogram images-82967\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetMaskImages()
        {
            string filePath = DDSMImages.ElementAt(0).DcomMaskFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00016_LEFT_CC_1\10-04-2016-DDSM-09887\1-cropped images-26184\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }
        
        [TestCase] //Here we have a path with two folders, so it activates the if else statements
        public void GetMaskImages2()
        {
            string filePath = DDSMImages.ElementAt(24).DcomMaskFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00145_LEFT_CC_1\09-27-2017-DDSM-31267\1-ROI mask images-45741\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]
        public void GetCroppedImages()
        {
            string filePath = DDSMImages.ElementAt(3).DcomCroppedFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00017_LEFT_MLO_1\10-04-2016-DDSM-27297\1-ROI mask images-18984\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }

        [TestCase]  //Here we have a path with two folders, so it activates the if else statements
        public void GetCroppedImages2()
        {
            string filePath = DDSMImages.ElementAt(24).DcomCroppedFilePath;
            string testValueResult = @"D:\Bryster\Mass-Test_P_00145_LEFT_CC_1\10-04-2016-DDSM-90238\1-cropped images-53302\000000.dcm";
            Assert.AreEqual(testValueResult, filePath);
        }



    }
}
