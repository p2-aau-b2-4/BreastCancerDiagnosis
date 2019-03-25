using System;
using System.Collections.Generic;

namespace LinearAlgebra 
{
  ///<summary>
  ///A vector
  ///</summary>
  Class Vector 
  {
    ///<summary>
    ///internal field containing all elements in the vector.
    ///</summary>
    private List<double> _elements;

    ///<summary>
    ///internal field containing the transposed state of the vector.
    ///</summary>
    private bool _transposed;

    ///<summary>
    ///Gets the elements of the vector as a List<double>
    ///</summary>
    public List<double> Elements { get; }
    ///<summary>
    ///Gets the transposed state of the vector
    ///</summary>
    public bool Transposed { get; }

    ///<summary>
    ///Initialises a new vector with one element.
    ///</summary>
    ///<param name=elem>the element the vector contains</param>
    ///<param name=transposed>transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Vector(double elem, bool transposed = false)
    {
      _transposed = transposed;
      _elements.Add(elem);
    }

    ///<summary>
    ///Initialises a new vector with any amount of elements.
    ///</summary>
    ///<param name=elems>A list of elements to initialise the vector from</param>
    ///<param name=transposed>transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Vector(IEnumerable<double> elems, bool transposed = false)
    {
      _transposed = transposed;
      _elements.AddRange(elems);
    }

    ///<summary>
    ///Initialises a new null vector
    ///</summary>
    ///<param name=transposed>transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Vector(bool transposed = false)
    {
      _transposed = transposed;
      _elements.Add(0);
    }

    ///<summary> 
    ///Marks the vector as transposed
    ///</summary>
    public void Transpose()
    {
      _transpose = _transpose == false ? true : false;
    }

  }
}
