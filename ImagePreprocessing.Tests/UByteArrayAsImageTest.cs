using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Dicom;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class UByteArrayAsImageTest
    {
        [TestCase]
        public void GetPngAsMemoryStreamTest()
        {
          var testImg =
                new Bitmap(Image.FromStream(DicomFile.Open("../../../000001.dcm").GetUByteImageInfo().GetPngAsMemoryStream()));
            var resultImg = new Bitmap(Image.FromFile("../../../000001.png"));
            var testImgData = testImg.LockBits(new Rectangle(0, 0, testImg.Width, testImg.Height),
                ImageLockMode.ReadWrite, testImg.PixelFormat);
            var resultImgData = resultImg.LockBits(new Rectangle(0, 0, resultImg.Width, resultImg.Height),
                ImageLockMode.ReadWrite, resultImg.PixelFormat);

            byte[] testData = new byte[testImgData.Height * testImgData.Stride];
            Marshal.Copy(testImgData.Scan0, testData, 0, testImgData.Height * testImgData.Stride);

            byte[] resultData = new byte[resultImgData.Height * resultImgData.Stride];
            Marshal.Copy(resultImgData.Scan0, resultData, 0, resultImgData.Height * resultImgData.Stride);

            bool equals = true;


            for (int i = 0; i < testData.Length; i++)
            {
                if (testData[i] != resultData[i] && testData[i] + 1 != resultData[i] &&
                    testData[i] - 1 != resultData[i])
                {
                    equals = false;
                }
            }

            Assert.True(equals);
        }

        [TestCase]
        public void SaveAsPNGTest()
        {

            var image = DicomFile.Open("../../../000001.dcm").GetUByteImageInfo();
            image.SaveAsPng("../../../test.png");

            var testImg = new Bitmap(Image.FromFile("../../../test.png"));
            var resultImg = new Bitmap(Image.FromFile("../../../000001.png"));

            var testImgData = testImg.LockBits(new Rectangle(0, 0, testImg.Width, testImg.Height),
                ImageLockMode.ReadWrite, testImg.PixelFormat);
            var resultImgData = resultImg.LockBits(new Rectangle(0, 0, resultImg.Width, resultImg.Height),
                ImageLockMode.ReadWrite, resultImg.PixelFormat);

            byte[] testData = new byte[testImgData.Height * testImgData.Stride];
            Marshal.Copy(testImgData.Scan0, testData, 0, testImgData.Height * testImgData.Stride);

            byte[] resultData = new byte[resultImgData.Height * resultImgData.Stride];
            Marshal.Copy(resultImgData.Scan0, resultData, 0, resultImgData.Height * resultImgData.Stride);

            bool equals = true;


            for (int i = 0; i < testData.Length; i++)
            {
                if (testData[i] != resultData[i] && testData[i] + 1 != resultData[i] &&
                    testData[i] - 1 != resultData[i])
                {
                    equals = false;
                }
            }

            Assert.True(equals);
        }


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
