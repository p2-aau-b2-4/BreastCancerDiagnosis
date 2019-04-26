using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Dicom;

namespace ImagePreprocessing.Tests
{
    class NormalizingUShortArrayExtensionTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [TestCase]
        public void ApplyContrastEnhancementTest10()
        {
            p

            CollectionAssert.AreEqual(pixelArrayTestValuesResult, image.PixelArray);
        }
    }
}
