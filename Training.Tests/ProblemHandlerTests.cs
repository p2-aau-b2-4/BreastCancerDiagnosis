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
            ushort[,] image1 = new ushort[,]
            {
                {1,2,3,4,5},
                {6,2,8,9,10},
                {11,8,2,10,11},
                {16,13,11,15,16},
                {21,2,1,1,25},
            };

            ushort[,] image2 = new ushort[,]
            {
                {1,2,3,4,5},
                {6,7,8,9,10},
                {11,8,9,10,11},
                {16,13,14,15,16},
                {21,2,23,24,25},
            };

            ImageWithResultModel resultImage1 = new ImageWithResultModel();
            resultImage1.Image = new UShortArrayAsImage(image1);
            resultImage1.Result = 1;
            
            
            ImageWithResultModel resultImage2 = new ImageWithResultModel();
            resultImage2.Image = new UShortArrayAsImage(image2);
            resultImage2.Result = 0;

            var images = new List<ImageWithResultModel>()
            {
                resultImage1,
                resultImage2
            };

            
            Pca pca = TrainingHelper.GetPca(images);
            
            SVMProblem problem = ProblemHandler.GetProblemFromImageModelResultList(images, pca,10);
            
            //Expected value
            SVMProblem realValue = new SVMProblem();
            
            realValue.Add(new SVMNode[]
            {
                new SVMNode(1, Double.NaN), 
                new SVMNode(2, Double.NaN), 
                new SVMNode(3, 1), 
                new SVMNode(4, Double.NaN), 
                new SVMNode(5, Double.NaN), 
                new SVMNode(6, Double.NaN), 
                new SVMNode(7, Double.NaN), 
                new SVMNode(8, Double.NaN), 
                new SVMNode(9, Double.NaN), 
                new SVMNode(10, Double.NaN), 
            }, 0);
            
            realValue.Add(new SVMNode[]
            {
                new SVMNode(1, Double.NaN), 
                new SVMNode(2, Double.NaN), 
                new SVMNode(3, 1), 
                new SVMNode(4, Double.NaN), 
                new SVMNode(5, Double.NaN), 
                new SVMNode(6, Double.NaN), 
                new SVMNode(7, Double.NaN), 
                new SVMNode(8, Double.NaN), 
                new SVMNode(9, Double.NaN), 
                new SVMNode(10, Double.NaN), 
            }, 1);

            Assert.AreEqual(realValue, problem);
            
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