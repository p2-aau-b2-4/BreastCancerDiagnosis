using NUnit.Framework;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class UByteArrayAsImageTest
    {
        [TestCase]
        public void UBytePixelArrayGetSetTest()
        {
            byte[,] pixelArray = new byte[,] { { 2, 50 }, { 0, 10 } };
            UByteArrayAsImage image = new UByteArrayAsImage(pixelArray);
            image.PixelArray = pixelArray;
            CollectionAssert.AreEqual(pixelArray, image.PixelArray);
        }
    }
}
