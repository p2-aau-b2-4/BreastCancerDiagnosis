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
        UshortArrayAsImage image;

       // UshortArrayAsImage testImage = { 0, 3, 6, 9 };

        [SetUp]
        public void Setup()
        {

            

        }

        [TestCase]
        public void SaveAsPNGTest()
        {
            var testImg = DicomFile.Open("000000.dcm");
            UshortArrayAsImage imgInfo = testImg.GetUshortImageInfo();
            Stream imgStream = imgInfo.GetPngAsMemoryStream();
            imgInfo.SaveAsPng("test.png");


            Bitmap finalImg = new Bitmap(imgStream);

            Image testimg2 = Image.FromFile("000000.png");

            Bitmap result = new Bitmap(testimg2);



            byte[] image1Bytes;
            byte[] image2Bytes;

            using (var mstream = new MemoryStream())
            {
                finalImg.Save(mstream, ImageFormat.Bmp);
                image1Bytes = mstream.ToArray();
            }

            using (var mstream = new MemoryStream())
            {
                result.Save(mstream, ImageFormat.Bmp);
                image2Bytes = mstream.ToArray();
            }

            string image1 = Convert.ToString(image1Bytes);
            string image2 = Convert.ToString(image2Bytes);


            Assert.AreEqual(image2, image1);

        }
    }
}
