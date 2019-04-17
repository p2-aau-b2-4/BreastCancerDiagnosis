using System;
using System.Collections.Generic;
using NUnit.Framework;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using DimensionReduction;

namespace DimensionReduction.Tests
{
  [TestFixture, Description("Tests for the PCA Class")]
  public class VectorTests
  {
    double floatingPointTolerance = 0.00000001;
    PCA p = new PCA();

    [Test, Description("Tests a normal case for MeanSubtraction")]
    public void MeanSubtractionNormalCaseTest()
    {
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

      SparseMatrix matrix = SparseMatrix.OfArray(matrixArray);
      SparseMatrix res = SparseMatrix.OfArray(resArray);
      p.MeanSubtraction(matrix);
      CollectionAssert.AreEqual(res.ToArray(),
          matrix.ToArray(),
          new Comparer(floatingPointTolerance));
    }

    [Test, Description("Tests the edge case where all values are 0 for MeanSubtraction")]
    public void MeanSubtractionEdgeCaseAllZeroTest()
    {
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

      SparseMatrix matrix = SparseMatrix.OfArray(matrixArray);
      SparseMatrix res = SparseMatrix.OfArray(resArray);
      p.MeanSubtraction(matrix);
      CollectionAssert.AreEqual(res.ToArray(),
          matrix.ToArray(),
          new Comparer(floatingPointTolerance));
    }

    [Test, Description("Tests edge case where values in a column sum up to infinity")]
    public void MeanSubtractionEdgeCaseLargeValuesTest()
    {
      double[,] matrixArray = new double[5,3] {
        {Double.MaxValue, Double.MaxValue, Double.MinValue},
        {Double.MaxValue, Double.MinValue, Double.MinValue},
        {Double.MaxValue, Double.MaxValue, Double.MinValue},
        {Double.MaxValue, Double.MinValue, Double.MinValue},
        {Double.MaxValue, Double.MaxValue, Double.MinValue}
      };

      SparseMatrix matrix = SparseMatrix.OfArray(matrixArray);
      Assert.Throws<NotFiniteNumberException>(() => p.MeanSubtraction(matrix));
    }

    [Test, Description("Tests a normal case for CovarianceMatrix")]
    public void CovarianceMatrixNormalCaseTest()
    {
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

      SparseMatrix matrix = SparseMatrix.OfArray(matrixArr);
      SparseMatrix expectationMatrix = SparseMatrix.OfArray(expectation);
      matrix = p.CovarianceMatrix(matrix);
      CollectionAssert.AreEqual(expectationMatrix.ToArray(),
          matrix.ToArray(),
          new Comparer(floatingPointTolerance));
    }
    [Test, Description("Tests another normal case for CovarianceMatrix")]
    public void CovarianceMatrixNormalCaseTwoTest()
    {
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

      double[,] expectation = new double[2,2] {
        {0.6165555556, 0.6154444444},
          {0.6154444444, 0.7165555556}
      };

      SparseMatrix matrix = SparseMatrix.OfArray(matrixArr);
      SparseMatrix expectationMatrix = SparseMatrix.OfArray(expectation);
      matrix = p.CovarianceMatrix(matrix);
      CollectionAssert.AreEqual(expectationMatrix.ToArray(),
          matrix.ToArray(),
          new Comparer(floatingPointTolerance));
    }
  }
}
