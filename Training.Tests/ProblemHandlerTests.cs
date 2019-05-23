using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            
            File.Delete(@"pca_model-100x100-CC-Train-MassCalc-BI.bin");
            
            PCA pca = TrainingHelper.GetPca(images);
            PCA pca2 = TrainingHelper.GetPca(images);
            
            SVMProblem problem = ProblemHandler.GetProblemFromImageModelResultList(images, pca,10);
            
            //Expected value
            SVMProblem realValue = new SVMProblem();
            
            realValue.Add(new SVMNode[]
            {
                new SVMNode(1, 20.056853498561878), 
                new SVMNode(2, 13.190302568584602), 
                new SVMNode(3, -1.0813980605883611), 
                new SVMNode(4, 0.38976510872122916), 
                new SVMNode(5, 8.8596355929840787), 
                new SVMNode(6, -7.3433006502883726), 
                new SVMNode(7, 10.837768344992746), 
                new SVMNode(8, 20.626727358988219), 
                new SVMNode(9, -1.7552480617394235), 
                new SVMNode(10, 25), 
            }, 0);
            
            realValue.Add(new SVMNode[]
            {
                new SVMNode(1, 22.292105243883533), 
                new SVMNode(2, 11.126898461982794), 
                new SVMNode(3, -2.3028333386433371), 
                new SVMNode(4, -6.2077696783291429), 
                new SVMNode(5, 12.172991455602181), 
                new SVMNode(6, -4.385545310384054), 
                new SVMNode(7, 13.837367251719812), 
                new SVMNode(8, 20.646721554636255), 
                new SVMNode(9, -1.8000434956830436), 
                new SVMNode(10, 25), 
            }, 1);
            
            bool fail = false;

            for (int i = 0; i < realValue.Y.Count; i++)
            {
                // Expected values
                var y = realValue.Y[i];
                var x = realValue.X[i];
                
                // Actual values
                var py = problem.Y[i];
                var px = problem.X[i];
                
                for (int j = 0; j < x.Length; j++)
                {
                    if (x[j].Value != px[j].Value)
                    {
                        fail = true;
                    }
                }
                
            }
            
            if (!fail)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
            
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