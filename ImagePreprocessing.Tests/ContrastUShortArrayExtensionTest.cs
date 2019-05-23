using NUnit.Framework;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class ContrastUShortArrayExtension
    {

        [TestCase]
        public void ApplyHistogramEqualizationTest()
        {

            var pixelArrayTestValuesResult = new ushort[,] { { 0, 21845 }, { 43690, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 0, 20001 }, { 40001, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }


        [TestCase]
        public void ApplyHistogramEqualizationTestWithTwoNumbersThatAreTheSame()
        {
            var pixelArrayTestValuesResult = new ushort[,] { { 0, 43690 }, { 43690, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 0, 20001 }, { 20001, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTestNumbersNotSequential()
        {
            var pixelArrayTestValuesResult = new ushort[,] { { 65535, 0 }, { 43690, 21845 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 65535, 0 }, { 40001, 20001 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTestWhiteImage()
        {
            var pixelArrayTestValuesResult = new ushort[,] { { 65535, 65535 }, { 65535, 65535 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 65535, 65535 }, { 65535, 65535 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyHistogramEqualizationTestBlackImage()
        {
            var pixelArrayTestValuesResult = new ushort[,] { { 0, 0 }, { 0, 0 } };
            var image = new UShortArrayAsImage(new ushort[,] { { 0, 0 }, { 0, 0 } });
            image = Contrast.ApplyHistogramEqualization(image);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }
    }
}
