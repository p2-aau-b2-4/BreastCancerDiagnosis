using System;
using System.Collections.Generic;
using LinearAlgebra;
using NUnit.Framework;

namespace LinearAlgebra.Test
{
  [TestFixture, Description("Tests for the Vector class")]
  public class VectorTests
  {

    [Test, Description("Is the null vector generated when no arguments are passed to Vector")]
    public void ConstructNullVectorValueIsZero()
    {
      Vector nullVector = new Vector();
      Assert.AreEqual(new List<double>() {0}, nullVector.Elements);
    }

    [Test, Description("Transposed state can be set to true")] 
    public void SetTransposedStateTrue()
    {
      Vector vector = new Vector(false);
      vector.Transpose();
      Assert.AreEqual(true, vector.Transposed);
    }

    [Test, Description("Transposed state can be set to false")] 
    public void SetTransposedStateFalse()
    {
      Vector vector = new Vector(true);
      vector.Transpose();
      Assert.AreEqual(false, vector.Transposed);
    }

    [Test, Description("Constructor works with 1 value")]
    public void ConstructSingleDimensionVector()
    {
      Vector vector = new Vector(double.MaxValue);
      Assert.AreEqual(new List<double>() {double.MaxValue}, vector.Elements);
    }

    [Test, Description("Constructor works with multiple values")]
    public void ConstructMultiDimensionalVector()
    {
      List<double> points = new List<double>() {double.MaxValue, double.MinValue, 0, -1, 1, 5.123, 682.3};
      Vector vector = new Vector(points);
      Assert.AreEqual(vector.Elements, points);
    }

    [Test, Description("Dimensions property returns the correct count")]
    public void DimensionsReturnsCount()
    {
      Vector vector = new Vector(new List<double>() {1, 2, 3, 4, 5});
      Assert.AreEqual(5, vector.Dimensions);
    }

  }
}
