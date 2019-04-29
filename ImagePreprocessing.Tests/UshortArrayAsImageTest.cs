using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using Dicom;



namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class UshortArrayAsImageTest
    {
        [TestCase]
        public void GetPngAsMemoryStreamTest()
        {
            var testImg = DicomFile.Open("000000.dcm");
            //testImg.RenderImage().AsBitmap().Save(@"test.jpg");
            
            UshortArrayAsImage testImgInfo = testImg.GetUshortImageInfo();
            Stream testImgStream = testImgInfo.GetPngAsMemoryStream();

            testImgInfo.SaveAsPng("test.png");

            Image finalImg = Image.FromFile("test.png");

            Bitmap finalImgBitmap = new Bitmap(finalImg);

            Image resultImg = Image.FromFile("000000.png");

            bool resultBool = Image.Equals(finalImg, resultImg);

            Bitmap resultImageBitmap = new Bitmap(resultImg);

            //string testImgString = finalImg.ToString();
            //string resultImgString = resultImageBitmap.ToString();

            byte[] testImgByteArr;
            byte[] resultImgByteArr;

            var mstreamTest = new MemoryStream();
            var mstreamResult = new MemoryStream();


            finalImgBitmap.Save(mstreamTest, ImageFormat.Png);
            testImgByteArr = mstreamTest.ToArray();

            resultImg.Save(mstreamResult, ImageFormat.Png);
            resultImgByteArr = mstreamResult.ToArray();


            // testImgByteArr = mstreamTest.ToArray();
            //resultImgByteArr = mstreamResult.ToArray();


            //string testImgString = Encoding.UTF8.GetString(testImgByteArr, 0, testImgByteArr.Length);
            //string resultImgString = Encoding.UTF8.GetString(resultImgByteArr, 0, resultImgByteArr.Length);

            string testImgString = BitConverter.ToString(testImgByteArr);
            string resultImgString = BitConverter.ToString(resultImgByteArr);

            //string testImgString = Convert.ToString(testImgByteArr);
            //string resultImgString = Convert.ToString(resultImgByteArr);

            //Assert.IsTrue(resultImgByteArr.SequenceEqual(testImgByteArr));
            Assert.AreEqual(resultImgString, testImgString);

        }
    }
}
