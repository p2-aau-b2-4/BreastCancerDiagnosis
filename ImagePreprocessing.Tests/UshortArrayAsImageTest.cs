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
            UshortArrayAsImage testImgInfo = testImg.GetUshortImageInfo();
            Stream testImgStream = testImgInfo.GetPngAsMemoryStream();


            Bitmap finalImg = new Bitmap(testImgStream);

            Image resultImage = Image.FromFile("000000.png");

            Bitmap resultImageBitmap = new Bitmap(resultImage);



            byte[] testImgByteArr;
            byte[] resultImgByteArr;

            var mstream = new MemoryStream();
            var mstream2 = new MemoryStream();

            testImgByteArr = mstream.ToArray();
            resultImgByteArr = mstream.ToArray();

            string testImgString = Convert.ToString(testImgByteArr);
            string resultImgString = Convert.ToString(resultImgByteArr);


            Assert.AreEqual(resultImgString, testImgString);

        }
    }
}
