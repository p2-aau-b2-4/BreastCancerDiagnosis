using System;

namespace DimensionReduction.Tests 
{
  public class Comparer : System.Collections.IComparer
  {
    private readonly double _tolerance;

    public Comparer(double tolerance)
    {
      _tolerance = tolerance;
    }

    public int Compare(object x, object y)
    {
      var a = (double)x;
      var b = (double)y;

      double delta = System.Math.Abs(a - b);
      if (delta < _tolerance)
      {
        return 0;
      }
      return a.CompareTo(b);
    }
  }
}
