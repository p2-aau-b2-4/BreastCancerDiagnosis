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
