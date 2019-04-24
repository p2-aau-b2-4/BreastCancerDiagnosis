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
        UshortArrayAsImage image;


        [SetUp]
        public void Setup()
        {
            pixelArray = new ushort[,] { { 0, 20001 }, { 40001, 65535 } };
            image = new UshortArrayAsImage(new byte[2 * 2 * 2], 2, 2);
        }

        [TestCase]
        public void ApplyContrastEnhancementTest10()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 0, 17320 }, { 41520, 65535 } };
            threshold = 10;
            image.PixelArray = pixelArray;
            image.ApplyContrastEnhancement(threshold);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyContrastEnhancementTest400()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 0, 0 }, { 65535, 65535 } };
            threshold = 400;
            image.PixelArray = pixelArray;
            image.ApplyContrastEnhancement(threshold);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTest()
        {
            pixelArrayTestValuesResult = new ushort[,] { { 16383, 32767 }, { 49151, 65535 } };
            image.PixelArray = pixelArray;
            image.ApplyHistogramEqualization();

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }










    }
}
