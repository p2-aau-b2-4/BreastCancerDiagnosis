using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dicom;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class ContrastUShortArrayExtension
    {
        ushort[,] pixelArray;
        ushort[,] pixelArrayTestValuesResult;
        double threshold;
        


        [SetUp]
        public void Setup()
        {
            pixelArray = new ushort[,] { { 0, 20001 }, { 40001, 65535 } };
           // image = new UShortArrayAsImage(new byte[2 * 2 * 2], 2, 2);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTest()
        {

            pixelArrayTestValuesResult = new ushort[,] { { 16383, 32767 }, { 49151, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 0, 20001 }, { 40001, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }


        [TestCase]
        public void ApplyHistogramEqualizationTestWithTwoNumbersThatAreTheSame()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 16383, 49151 }, { 49151, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 0, 20001 }, { 20001, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTestNumbersNotSequential()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 65535, 16383 }, { 49151, 32767 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 65535, 0 }, { 40001, 20001 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTestBlackImage()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 65535, 65535 }, { 65535, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 65535, 65535 }, { 65535, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

    }
}
