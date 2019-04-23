using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NUnit.Framework;
using Dicom;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class UshortArrayAsImageTest
    {
        UshortArrayAsImage image;

       // UshortArrayAsImage testImage = { 0, 3, 6, 9 };

        [SetUp]
        public void Setup()
        {

            

        }

        [TestCase]
        public void SaveAsPNGTest()
        {
            var testimg = DicomFile.Open("e.dcm");
            var testResult = testimg.GetHashCode();
            UshortArrayAsImage imgInfo = testimg.GetUshortImageInfo();
            imgInfo.SaveAsPng("test.PNG");
            // var result = imgInfo.GetHashCode();

            var testimg2 = Image.FromFile("test.png");
            var result = testimg2.GetHashCode();

            Assert.AreEqual(result, testResult);

            //tror at jeg skal konvertere til bitmap før jeg bruger gethashcode
        }
    }
}
