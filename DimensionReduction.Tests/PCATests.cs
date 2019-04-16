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
      CollectionAssert.AreEqual(res.ToArray(), matrix.ToArray(), new Comparer(floatingPointTolerance));
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
      CollectionAssert.AreEqual(res.ToArray(), matrix.ToArray(), new Comparer(floatingPointTolerance));
    }

    [Test, Description("Tests edge case where values in a column sum up to infinity")]
    public void MeanSubtractionEdgeCaseLargeValuesTest()
    {
      double[,] matrixArray = new double[3,5] {
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
      double [,] matrix = new matrix[,]
    }
  }
}
