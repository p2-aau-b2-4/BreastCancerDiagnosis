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
    PCA p = new PCA();

    [Test, Description("Tests a normal case for MeanSubtraction")]
    public void MeanSubtractionTest()
    {
      double[,] matrixArray = new double[5,5] {
        {2.0, 3.4, 0.0, 2.9, 5.1},
        {3.1, 9.2, 7.9, -2.3, 1.0},
        {-6.1, -5.7, 1.0, -8.3, 3.3},
        {9.9, -0.3, 4.0, -4.0, 2.8},
        {3.6, 7.6, -2.3, 7.6, -8.3}
      };

      double[,] resArray = new double[5,5] {
        {-0.68, 0.72, -2.68, 0.22, 2.42},
        {-0.68, 5.42, 4.12, -6.08, -2.78},
        {-2.94, -2.54, 4.16, -5.14, 6.46},
        {7.40, -2.80, -1.50, -6.50, 0.40},
        {1.96, 5.96, -3.94, 5.96, -9.94}
      };

      SparseMatrix matrix = new SparseMatrix.FromArray(matrixArray);
      SparseMatrix res = new SparseMatrix.FromArray(resArray);
      p.MeanSubtraction(matrix);
      Assert.AreEqual(matrix, res);
    }
  }
}
