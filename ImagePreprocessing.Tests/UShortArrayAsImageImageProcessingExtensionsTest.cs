using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dicom;

namespace ImagePreprocessing.Tests
{
    [TestFixture]
    class UShortArrayAsImageImageProcessingExtensionsTest
    {
        [TestCase]
        public void ApplyContrastEnhancementTest10()
        {

            ushort[,] pixelArray = { { 0, 20001 }, { 40001, 65535 } };
            ushort[,] pixelArrayTestValuesResult = { { 0, 17320 }, { 41520, 65535 } };

            double threshold = 10;
            var img = DicomFile.Open("e.dcm");
            UshortArrayAsImage image = new UshortArrayAsImage(new byte[2*2*2], 2, 2);
            image.PixelArray = pixelArray;
            
            image.ApplyContrastEnhancement(threshold);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }

        [TestCase]
        public void ApplyContrastEnhancementTest400()
        {

            ushort[,] pixelArray = { { 0, 20001 }, { 40001, 65535 } };
            ushort[,] pixelArrayTestValuesResult = { { 0, 0 }, { 65535, 65535 } };

            double threshold = 400;
            var img = DicomFile.Open("e.dcm");
            UshortArrayAsImage image = new UshortArrayAsImage(new byte[2 * 2 * 2], 2, 2);
            image.PixelArray = pixelArray;

            image.ApplyContrastEnhancement(threshold);

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }


        


    }
}
