using System;
using System.Collections.Generic;
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

            images.Add(ddsmImageMock.Object);

            List<ImageWithResultModel> result = ProblemHandler.TransformDdsmImageList(images);

        }

        [TestCase]
        public void TrainAndTestSvmTest()
        {
            SVMProblem train = new SVMProblem();
            Random random = new Random();
            for (int i = 0; i < 30; i++)
            {
                int value = random.Next() % 2;
                train.Add(new SVMNode[]
                {
                    new SVMNode(1,value*4),
                    new SVMNode(2,random.Next()), 
                    new SVMNode(3,random.Next()), 
                    new SVMNode(4,random.Next()), 
                    new SVMNode(5,random.Next()), 
                    new SVMNode(6,random.Next()), 
                    new SVMNode(7,random.Next()), 
                    new SVMNode(8,random.Next()), 
                }, value);
            }
            
            SVMProblem test = new SVMProblem();
            for (int i = 0; i < 10; i++)
            {
                int value = random.Next() % 2;
                train.Add(new SVMNode[]
                {
                    new SVMNode(1,value*4),
                    new SVMNode(2,random.Next()), 
                    new SVMNode(3,random.Next()), 
                    new SVMNode(4,random.Next()), 
                    new SVMNode(5,random.Next()), 
                    new SVMNode(6,random.Next()), 
                    new SVMNode(7,random.Next()), 
                    new SVMNode(8,random.Next()), 
                }, value);
            }

            ProblemHandler.SvmResult result = ProblemHandler.TrainAndTestSvm(train, test);
            Assert.True(result.TestAccuracy > 95);
        }
}
}