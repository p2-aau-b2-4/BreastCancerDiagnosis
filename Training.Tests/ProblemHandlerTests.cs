using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DimensionReduction;
using ImagePreprocessing;
using LibSVMsharp;
using Moq;
using NUnit.Framework;

namespace Training.Tests
{
    [TestFixture]
    public class ProblemHandlerTests
    {
        [TestCase]
        public void TransformDdsmImageListTest()
        {
            var images = new List<DdsmImage>();

            var ddsmImageMock = new Mock<DdsmImage>();
            ushort[,] imageBefore = new ushort[,]
            {
                {1,2,3,4,5},
                {6,7,8,9,10},
                {11,8,9,10,11},
                {16,13,14,15,16},
                {21,2,23,24,25},
            };
            
            byte[,] maskBefore = new byte[,]
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,255,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
            };
            
            ddsmImageMock.SetupGet(x => x.DcomOriginalImage).Returns(new UShortArrayAsImage(imageBefore));
            ddsmImageMock.SetupGet(x => x.DcomMaskImage).Returns(new UByteArrayAsImage(maskBefore));
            
            images.Add(ddsmImageMock.Object);

            var result = ProblemHandler.TransformDdsmImageList(images).First();
            bool foundError = false;
            for (int i = 0; i < result.Image.Height; i++)
            {
                for (int u = 0; u < result.Image.Height; u++)
                {
                    if (result.Image.PixelArray[i, u] != UInt16.MaxValue)
                    {
                        Console.WriteLine($"{i},{u} failed, was {result.Image.PixelArray[i,u]}");
                        foundError = true;
                    }
                }
            }

            Assert.True(!foundError);

        }

        [TestCase]
        public void GetProblemFromImageModelResultListTest()
        {
            var images = new List<ImageWithResultModel>();

            var ddsmImageMock = new Mock<ImageWithResultModel>();
            ushort[,] imageBefore = new ushort[,]
            {
                {1,2,3,4,5},
                {6,7,8,9,10},
                {11,8,9,10,11},
                {16,13,14,15,16},
                {21,2,23,24,25},
            };
            
            ddsmImageMock.SetupGet(x => x.Image).Returns(new UShortArrayAsImage(imageBefore));
            
            images.Add(ddsmImageMock.Object);

            PCA pca = TrainingHelper.GetPca(images);
            
            ProblemHandler.GetProblemFromImageModelResultList(images, pca,10);

        }

        [TestCase]
        public void TrainAndTestSvmTest()
        {
            SVMProblem train = new SVMProblem();
            Random random = new Random();
            for (int i = 0; i < 300; i++)
            {
                int value = random.Next() % 2;
                train.Add(new SVMNode[]
                {
                    new SVMNode(1,value),
                }, value);
            }
            
            SVMProblem test = new SVMProblem();
            for (int i = 0; i < 100; i++)
            {
                int value = random.Next() % 2;
                test.Add(new SVMNode[]
                {
                    new SVMNode(1,value),
                }, value);
            }

            ProblemHandler.SvmResult result = ProblemHandler.TrainAndTestSvm(train, test);
            Assert.True(result.TestAccuracy > 95);
        }
}
}