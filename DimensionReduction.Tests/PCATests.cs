using System;
using Accord.Math;
using Accord.Statistics;
using NUnit.Framework;

namespace DimensionReduction.Tests
{
    [TestFixture, Description("Tests for the PCA Class")]
    public class PCATests
    {
        double floatingPointTolerance = 0.0000001;

        [Test, Description("Tests a normal case for MeanSubtraction")]
        public void MeanSubtractionNormalCaseTest()
        {
            PCA p = new PCA();
            double[,] matrixArray = new double[5,5] {
                    {2.0, 3.4, 0.0, 2.9, 5.1},
                    {3.1, 9.2, 7.9, -2.3, 1.0},
                    {-6.1, -5.7, 1.0, -8.3, 3.3},
                    {9.9, -0.3, 4.0, -4.0, 2.8},
                    {3.6, 7.6, -2.3, 7.6, -8.3}
            };

            double[,] resArray = new double[5,5] {
                {-0.50, 0.56, -2.12, 3.72, 4.32},
                    {0.60, 6.36, 5.78, -1.48, 0.22},
                    {-8.60, -8.54, -1.12, -7.48, 2.52},
                    {7.40, -3.14, 1.88, -3.18, 2.02},
                    {1.10, 4.76, -4.42, 8.42, -9.08}
            };

            matrixArray = p.MeanSubtraction(matrixArray);
            CollectionAssert.AreEqual(resArray,
                    matrixArray,
                    new Comparer(floatingPointTolerance));
        }
        
        [Test, Description("Tests a normal case non square for MeanSubtraction")]
        public void MeanSubtractionNormalCaseTestNonSquare()
        {
            PCA p = new PCA();
            double[,] matrixArray = new double[3,5] {
                {2.0, 3.4, 0.0, 2.9, 5.1},
                {3.1, 9.2, 7.9, -2.3, 1.0},
                {-6.1, -5.7, 1.0, -8.3, 3.3}
            };

            double[,] resArray = new double[3,5] {
                {2.3333333333, 1.1, -2.9666666666, 5.4666666666, 1.9666666666},
                {3.433333333, 6.9, 4.93333333333, 0.2666666666, -2.1333333333},
                {-5.766666666, -8.00, -1.9666666666, -5.733333333, 0.1666666666}
            };

            matrixArray = p.MeanSubtraction(matrixArray);
            CollectionAssert.AreEqual(resArray,
                matrixArray,
                new Comparer(floatingPointTolerance));
        }

        [Test, Description("Tests the edge case where all values are 0 for MeanSubtraction")]
        public void MeanSubtractionEdgeCaseAllZeroTest()
        {
            PCA p = new PCA();
            double[,] matrixArray = new double[3,3] {
                {0.0, 0.0, 0.0},
                    {0.0, -0.0, 0.0},
                    {-0.0, 0.0, -0.0}
            };

            double[,] resArray = new double[3,3] {
                {0.0, 0.0, 0.0},
                    {0.0, 0.0, 0.0},
                    {0.0, 0.0, 0.0}
            };

            matrixArray = p.MeanSubtraction(matrixArray);
            CollectionAssert.AreEqual(resArray,
                    matrixArray,
                    new Comparer(floatingPointTolerance));
        }

        [Test, Description("Tests edge case where values in a column sum up to infinity")]
        public void MeanSubtractionEdgeCaseLargeValuesTest()
        {
            PCA p = new PCA();
            double[,] matrixArray = new double[5,3] {
                {Double.MaxValue, Double.MaxValue, Double.MinValue},
                    {Double.MaxValue, Double.MinValue, Double.MinValue},
                    {Double.MaxValue, Double.MaxValue, Double.MinValue},
                    {Double.MaxValue, Double.MinValue, Double.MinValue},
                    {Double.MaxValue, Double.MaxValue, Double.MinValue}
            };

            Assert.Throws<NotFiniteNumberException>(() => p.MeanSubtraction(matrixArray));
        }

        [Test, Description("Tests a normal case non-square matrix with more rows than columns for CovarianceMatrix")]
        public void CovarianceMatrixNormalCaseNonSquareMoreRowsTest()
        {
            PCA p = new PCA();
            double[,] matrixArr = new double[10,2] {
                    {0.69, 0.49},
                    {-1.31, -1.21},
                    {0.39, 0.99},
                    {0.09, 0.29},
                    {1.29, 1.09},
                    {0.49, 0.79},
                    {0.19, -0.31},
                    {-0.81, -0.81},
                    {-0.31, -0.31},
                    {-0.71, -1.01}
            };

            double[,] expectation = new double[2,2] {
                    {0.6165555556, 0.6154444444},
                    {0.6154444444, 0.7165555556}
            };

            matrixArr = p.CovarianceMatrix(matrixArr);
            CollectionAssert.AreEqual(expectation,
                    matrixArr,
                    new Comparer(floatingPointTolerance));
        }
        
        [Test, Description("Tests a normal case non-square matrix with more rows than columns for CovarianceMatrix")]
        public void CovarianceMatrixNormalCaseNonSquareMoreRowsTest2()
        {
            PCA p = new PCA();
            double[,] matrixArr = new double[10,2] 
            {
                {1.507, 0.988},
                {2.107, -9.312},
                {1.407, 1.798},
                {1.397, 2.098},
                {-9.563, 1.988},
                {0.797, 0.888},
                {2.607, 0.488},
                {-0.493, 1.588},
                {0.627, -0.412},
                {-0.393, -0.112}
            };
            
            double[,] expectation = new double[2,2] {
                {12.25722333, -3.41102889},
                {-3.41102889, 11.44519556}
            };
            
            matrixArr = p.CovarianceMatrix(matrixArr);
            CollectionAssert.AreEqual(expectation,
                matrixArr,
                new Comparer(floatingPointTolerance));
            
        }

        [Test, Description("Tests a normal case non-square matrix with more columns than rows for CovarianceMatrix")]
        public void CovarianceMatrixNormalCaseNonSquareMoreColumnsTest()
        {
            PCA p = new PCA();
            double[,] matrixArr = new double[2,10] {
                    {0.69, 0.49, -1.31, -1.21, 0.39, 0.99, 0.09, 0.29, 1.29, 1.09},
                    {0.49, 0.79, 0.19, -0.31, -0.81, -0.81, -0.31, -0.31, -0.71, -1.01}
            };

            double[,] expectation = new double[10,10];

            expectation[0,0] = 0.199999999999999900e-1;
            expectation[0,1] = -0.299999999999999989e-1;
            expectation[0,2] = -0.149999999999999967e0;
            expectation[0,3] = -0.899999999999999689e-1;
            expectation[0,4] = 0.119999999999999996e0;
            expectation[0,5] = 0.179999999999999966e0;
            expectation[0,6] = 0.399999999999999939e-1;
            expectation[0,7] = 0.599999999999999839e-1;
            expectation[0,8] = 0.199999999999999956e0;
            expectation[0,9] = 0.209999999999999964e0;
            expectation[1,0] = -0.299999999999999989e-1;
            expectation[1,1] = 0.450000000000000122e-1;
            expectation[1,2] = 0.225000000000000033e0;
            expectation[1,3] = 0.135000000000000009e0;
            expectation[1,4] = -0.180000000000000049e0;
            expectation[1,5] = -0.270000000000000073e0;
            expectation[1,6] = -0.600000000000000117e-1;
            expectation[1,7] = -0.900000000000000105e-1;
            expectation[1,8] = -0.300000000000000044e0;
            expectation[1,9] = -0.315000000000000058e0;
            expectation[2,0] = -0.149999999999999967e0;
            expectation[2,1] = 0.225000000000000033e0;
            expectation[2,2] = 0.112500000000000000e1;
            expectation[2,3] = 0.675000000000000044e0;
            expectation[2,4] = -0.900000000000000133e0;
            expectation[2,5] = -0.135000000000000009e1;
            expectation[2,6] = -0.300000000000000044e0;
            expectation[2,7] = -0.449999999999999956e0;
            expectation[2,8] = -0.150000000000000000e1;
            expectation[2,9] = -0.157500000000000018e1;
            expectation[3,0] = -0.899999999999999689e-1;
            expectation[3,1] = 0.135000000000000009e0;
            expectation[3,2] = 0.675000000000000044e0;
            expectation[3,3] = 0.404999999999999971e0;
            expectation[3,4] = -0.540000000000000036e0;
            expectation[3,5] = -0.810000000000000053e0;
            expectation[3,6] = -0.179999999999999993e0;
            expectation[3,7] = -0.270000000000000018e0;
            expectation[3,8] = -0.899999999999999911e0;
            expectation[3,9] = -0.945000000000000062e0;
            expectation[4,0] = 0.119999999999999996e0;
            expectation[4,1] = -0.180000000000000049e0;
            expectation[4,2] = -0.900000000000000133e0;
            expectation[4,3] = -0.540000000000000036e0;
            expectation[4,4] = 0.720000000000000195e0;
            expectation[4,5] = 0.108000000000000029e1;
            expectation[4,6] = 0.240000000000000047e0;
            expectation[4,7] = 0.360000000000000042e0;
            expectation[4,8] = 0.120000000000000018e1;
            expectation[4,9] = 0.126000000000000023e1;
            expectation[5,0] = 0.179999999999999966e0;
            expectation[5,1] = -0.270000000000000073e0;
            expectation[5,2] = -0.135000000000000009e1;
            expectation[5,3] = -0.810000000000000053e0;
            expectation[5,4] = 0.108000000000000029e1;
            expectation[5,5] = 0.162000000000000011e1;
            expectation[5,6] = 0.360000000000000042e0;
            expectation[5,7] = 0.540000000000000036e0;
            expectation[5,8] = 0.180000000000000004e1;
            expectation[5,9] = 0.189000000000000012e1;
            expectation[6,0] = 0.399999999999999939e-1;
            expectation[6,1] = -0.600000000000000117e-1;
            expectation[6,2] = -0.300000000000000044e0;
            expectation[6,3] = -0.179999999999999993e0;
            expectation[6,4] = 0.240000000000000047e0;
            expectation[6,5] = 0.360000000000000042e0;
            expectation[6,6] = 0.800000000000000155e-1;
            expectation[6,7] = 0.119999999999999996e0;
            expectation[6,8] = 0.400000000000000022e0;
            expectation[6,9] = 0.420000000000000040e0;
            expectation[7,0] = 0.599999999999999839e-1;
            expectation[7,1] = -0.900000000000000105e-1;
            expectation[7,2] = -0.449999999999999956e0;
            expectation[7,3] = -0.270000000000000018e0;
            expectation[7,4] = 0.360000000000000042e0;
            expectation[7,5] = 0.540000000000000036e0;
            expectation[7,6] = 0.119999999999999996e0;
            expectation[7,7] = 0.179999999999999993e0;
            expectation[7,8] = 0.599999999999999978e0;
            expectation[7,9] = 0.630000000000000004e0;
            expectation[8,0] = 0.199999999999999956e0;
            expectation[8,1] = -0.300000000000000044e0;
            expectation[8,2] = -0.150000000000000000e1;
            expectation[8,3] = -0.899999999999999911e0;
            expectation[8,4] = 0.120000000000000018e1;
            expectation[8,5] = 0.180000000000000004e1;
            expectation[8,6] = 0.400000000000000022e0;
            expectation[8,7] = 0.599999999999999978e0;
            expectation[8,8] = 0.2e1;
            expectation[8,9] = 0.210000000000000009e1;
            expectation[9,0] = 0.209999999999999964e0;
            expectation[9,1] = -0.315000000000000058e0;
            expectation[9,2] = -0.157500000000000018e1;
            expectation[9,3] = -0.945000000000000062e0;
            expectation[9,4] = 0.126000000000000023e1;
            expectation[9,5] = 0.189000000000000012e1;
            expectation[9,6] = 0.420000000000000040e0;
            expectation[9,7] = 0.630000000000000004e0;
            expectation[9,8] = 0.210000000000000009e1;
            expectation[9,9] = 0.220500000000000007e1;
            
            double[,] cpyMatrix = new double[2,10];
            matrixArr.CopyTo(cpyMatrix);
            
            matrixArr = p.MeanSubtraction(matrixArr);
            matrixArr = p.CovarianceMatrix(matrixArr);
            CollectionAssert.AreEqual(expectation,
                    matrixArr,
                    new Comparer(floatingPointTolerance));

            double[,] ourCov = matrixArr;
            double[,] cov = cpyMatrix.Covariance();
            CollectionAssert.AreEqual(expectation,cov, new Comparer(floatingPointTolerance));
        }
        [Test, Description("Tests another normal square case for CovarianceMatrix")]
        public void CovarianceMatrixNormalCaseSquareTest()
        {
            PCA p = new PCA();
            double[,] matrixArr = new double[9, 9] {
                    {-3617,7121,-1770,-3850,-5723,8288,1787,5367,-1375},
                    {-1733,722,946,5770,-399,-5187,-4681,9403,3872},
                    {-3925,-1839,6520,2019,6463,-8250,-6075,-1832,2983},
                    {6100,-3915,-1382,-9308,-4979,8051,-533,5281,9673},
                    {1243,-919,9893,-3647,-2795,1933,4824,33,8109},
                    {-6118,-9715,8984,-1367,4956,6600,-4139,-2693,4956},
                    {4136,-664,5962,4935,-6293,8180,-5666,-7926,9214},
                    {2202,8521,-4741,5402,9037,4987,-4376,-7846,4644},
                    {-3823,-1998,-3350,570,-7444,8243,7435,2623,6424}
            };

            double[,] expectation = new double[9,9];

            // array generated by maple
            expectation[0,0] = 0.176053975000000000e8;
            expectation[0,1] = 0.457826837500000000e7;
            expectation[0,2] = -0.399328887499999953e7;
            expectation[0,3] = -0.331221700000000093e7;
            expectation[0,4] = -0.627830337500000000e7;
            expectation[0,5] = 0.7003523e7;
            expectation[0,6] = -0.186999875000000000e7;
            expectation[0,7] = -0.4798844e7;
            expectation[0,8] = 0.986533037500000000e7;
            expectation[1,0] = 0.457826837500000000e7;
            expectation[1,1] = 0.301754470277777761e8;
            expectation[1,2] = -0.182812283888888881e8;
            expectation[1,3] = 0.848513673611111008e7;
            expectation[1,4] = 0.186260465277778334e7;
            expectation[1,5] = 0.428261597222222248e6;
            expectation[1,6] = 0.823882833333333256e6;
            expectation[1,7] = -0.153511636111111194e7;
            expectation[1,8] = -0.885727530555555597e7;
            expectation[2,0] = -0.399328887499999953e7;
            expectation[2,1] = -0.182812283888888881e8;
            expectation[2,2] = 0.308733886944444440e8;
            expectation[2,3] = -0.117307893055555481e7;
            expectation[2,4] = 0.462695686111111008e7;
            expectation[2,5] = -0.106785538611111101e8;
            expectation[2,6] = -0.589139766666666698e7;
            expectation[2,7] = -0.852483606944444589e7;
            expectation[2,8] = 0.435253290277777892e7;
            expectation[3,0] = -0.331221700000000093e7;
            expectation[3,1] = 0.848513673611111008e7;
            expectation[3,2] = -0.117307893055555481e7;
            expectation[3,3] = 0.259787679444444478e8;
            expectation[3,4] = 0.120599827361111119e8;
            expectation[3,5] = -0.124263529861111119e8;
            expectation[3,6] = -0.124783096666666660e8;
            expectation[3,7] = -0.119806404444444440e8;
            expectation[3,8] = -0.266636397222222295e7;
            expectation[4,0] = -0.627830337500000000e7;
            expectation[4,1] = 0.186260465277778334e7;
            expectation[4,2] = 0.462695686111111008e7;
            expectation[4,3] = 0.120599827361111119e8;
            expectation[4,4] = 0.378507245277777761e8;
            expectation[4,5] = -0.191299344027777798e8;
            expectation[4,6] = -0.184137604166666679e8;
            expectation[4,7] = -0.155837438611111119e8;
            expectation[4,8] = -0.572766680555555504e7;
            expectation[5,0] = 0.7003523e7;
            expectation[5,1] = 0.428261597222222248e6;
            expectation[5,2] = -0.106785538611111101e8;
            expectation[5,3] = -0.124263529861111119e8;
            expectation[5,4] = -0.191299344027777798e8;
            expectation[5,5] = 0.394546397777777761e8;
            expectation[5,6] = 0.127166160416666679e8;
            expectation[5,7] = -0.693834988888888806e7;
            expectation[5,8] = 0.558211630555555597e7;
            expectation[6,0] = -0.186999875000000000e7;
            expectation[6,1] = 0.823882833333333256e6;
            expectation[6,2] = -0.589139766666666698e7;
            expectation[6,3] = -0.124783096666666660e8;
            expectation[6,4] = -0.184137604166666679e8;
            expectation[6,5] = 0.127166160416666679e8;
            expectation[6,6] = 0.243410542500000000e8;
            expectation[6,7] = 0.108749302916666660e8;
            expectation[6,8] = 0.144274745833333372e7;
            expectation[7,0] = -0.4798844e7;
            expectation[7,1] = -0.153511636111111194e7;
            expectation[7,2] = -0.852483606944444589e7;
            expectation[7,3] = -0.119806404444444440e8;
            expectation[7,4] = -0.155837438611111119e8;
            expectation[7,5] = -0.693834988888888806e7;
            expectation[7,6] = 0.108749302916666660e8;
            expectation[7,7] = 0.357919496944444403e8;
            expectation[7,8] = -0.550449015277777798e7;
            expectation[8,0] = 0.986533037500000000e7;
            expectation[8,1] = -0.885727530555555597e7;
            expectation[8,2] = 0.435253290277777892e7;
            expectation[8,3] = -0.266636397222222295e7;
            expectation[8,4] = -0.572766680555555504e7;
            expectation[8,5] = 0.558211630555555597e7;
            expectation[8,6] = 0.144274745833333372e7;
            expectation[8,7] = -0.550449015277777798e7;
            expectation[8,8] = 0.120046551111111119e8;

            matrixArr = p.MeanSubtraction(matrixArr);
            matrixArr = p.CovarianceMatrix(matrixArr);
            CollectionAssert.AreEqual(expectation,
                    matrixArr,
                    new Comparer(floatingPointTolerance));
        }
        
        [Test, Description("Tests PCA training where number of components is tested")]
        public void TrainNumberOfComponents()
        {
            PCA p = new PCA();
            
            double[,] matrix2 = new double[10,2] {
                {1.507, 0.988},
                {2.107, -9.312},
                {1.407, 1.798},
                {1.397, 2.098},
                {-9.563, 1.988},
                {0.797, 0.888},
                {2.607, 0.488},
                {-0.493, 1.588},
                {0.627, -0.412},
                {-0.393, -0.112}
            };

            int expectation = 2; 
            
            p.Train(matrix2);
            Assert.AreEqual(expectation,p.ComponentVectors.Length);
        }
        
        [Test, Description("Tests PCA training in a normal case")]
        public void TrainNormalCase()
        {
            PCA p = new PCA();
            double[,] matrix = new double[10,2] {
                {0.69, 0.49},
                {-1.31, -1.21},
                {0.39, 0.99},
                {0.09, 0.29},
                {1.29, 1.09},
                {0.49, 0.79},
                {0.19, -0.31},
                {-0.81, -0.81},
                {-0.31, -0.31},
                {-0.71, -1.01}
            };
            
            double[] expectation = {0.6778734, 0.73517866}; // Verified with octave
            
            p.Train(matrix);
            CollectionAssert.AreEqual(expectation,
                p.ComponentVectors[0],
                new Comparer(floatingPointTolerance));
        }
        
        [Test, Description("Tests PCA training in a normal case jagged array")]
        public void TrainNormalCaseJaggedArray()
        {
            PCA p = new PCA();
            double[][] matrix = new double[10][];
            matrix[0] = new double[] {0.69, 0.49};
            matrix[1] = new double[] {-1.31, -1.21};
            matrix[2] = new double[] {0.39, 0.99};
            matrix[3] = new double[] {0.09, 0.29};
            matrix[4] = new double[] {1.29, 1.09};
            matrix[5] = new double[] {0.49, 0.79};
            matrix[6] = new double[] {0.19, -0.31};
            matrix[7] = new double[] {-0.81, -0.81};
            matrix[8] = new double[] {-0.31, -0.31};
            matrix[9] = new double[] {-0.71, -1.01};
            
            double[] expectation = {0.6778734, 0.73517866}; // Verified with octave
            
            p.Train(matrix);
            CollectionAssert.AreEqual(expectation,
                p.ComponentVectors[0],
                new Comparer(floatingPointTolerance));
        }
        
        [Test, Description("Tests GetComponentFromImage in a normal case")]
        public void GetComponentFromImageNormalCase() // Not working 
        {
            PCA p = new PCA();
            double[,] matrixArr = new double[10,2] 
            {
                {1.507, 0.988},
                {2.107, -9.312},
                {1.407, 1.798},
                {1.397, 2.098},
                {-9.563, 1.988},
                {0.797, 0.888},
                {2.607, 0.488},
                {-0.493, 1.588},
                {0.627, -0.412},
                {-0.393, -0.112}
            };
            
            double[] expectation = new double[1] {0.083723606};
            
            double[,] img = new double[1,2] {{1,1}};

            p.Train(matrixArr);
            double[] res = p.GetComponentsFromImage(img, 1);
            Console.WriteLine(res.Length);
            CollectionAssert.AreEqual(expectation,
                res,
                new Comparer(floatingPointTolerance));
        }
    }
}
