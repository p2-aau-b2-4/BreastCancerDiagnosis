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
            UshortArrayAsImage imgInfo = testImg.GetUshortImageInfo();
            Stream imgStream = imgInfo.GetPngAsMemoryStream();


            Bitmap finalImg = new Bitmap(imgStream);

            Image testimg2 = Image.FromFile("000000.png");

            Bitmap result = new Bitmap(testimg2);



            byte[] image1Bytes;
            byte[] image2Bytes;

            var mstream = new MemoryStream();
            var mstream2 = new MemoryStream();

            image1Bytes = mstream.ToArray();
            image2Bytes = mstream.ToArray();

            string image1 = Convert.ToString(image1Bytes);
            string image2 = Convert.ToString(image2Bytes);


            Assert.AreEqual(image2, image1);

        }
    }
}
