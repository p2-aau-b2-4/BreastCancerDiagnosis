using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using NUnit.Framework;
using Dicom;
using System.Windows;
using static ImagePreprocessing.Normalization;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class NormalizationTest
    {
        [TestCase]
        public void GetTumourPositionFromMaskTest()
        {
            Rectangle RealValue = new Rectangle(1, 1, 1, 1);
            byte[,] testValue = new byte[,] { { 0, 0, 0 }, { 0, 255, 0 }, { 0, 0, 0 } };
            UByteArrayAsImage image = new UByteArrayAsImage(testValue);
            Rectangle rectangleTestValue = GetTumourPositionFromMask(image);

            Assert.AreEqual(RealValue, rectangleTestValue);
        }
    }
}
