using System;
using System.Collections.Generic;

namespace LinearAlgebra 
{
  Class Vector 
  {
    private List<double> _elements;
    private bool _transposed;

    public List<double> Elements { get; }
    public bool Transposed { get; }

    public Vector(double elem)
    {
      _transposed = false;
      _elements.Add(elem);
    }

    public Vector(

  }
}
