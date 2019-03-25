using System;
using System.Collections.Generic;

namespace LinearAlgebra 
{
  ///<summary>
  ///A sparse representation matrix (CSR)
  ///</summary>
  public class Matrix
  {

    ///<summary>
    ///Denotes the number of columns in a matrix.
    ///</summary>
    private int _n;

    ///<summary>
    ///Denotes the number of rows in a matrix.
    ///</summary>
    private int _m;

    ///<summary>
    ///Stores the values of the non-zero elements of the matrix.
    ///</summary>
    private Vector _A;
    ///<summary>
    ///Stores the cumulative number of non-zero elements up to (not including)
    ///the ith row. Defined by the recursive relation:
    ///IA[0] = 0
    ///IA[i] = IA[i-1] + num of non-zero elements in the (i-1)th row.
    ///</summary>
    private Vector _IA;
    ///<summary>
    ///Stores the column index of each element in _A
    ///</summary>
    private Vector _JA;

    ///<summary>
    ///Nonzero elements in the matrix.
    ///</summary>
    private int NNZ;

    ///<summary>
    ///internal field containing the transposed state of the Matrix.
    ///</summary>
    private bool _transposed;

    ///<summary>
    ///Gets the elements of the vector as a List<double>
    ///</summary>
    public List<double> Elements { get; }
    ///<summary>
    ///Gets the transposed state of the vector. This is only relevant when doing
    ///Matrix-Vector calculations.
    ///</summary>
    public bool Transposed { get; }

    public int Dimensions
    {
      get
      {
	return _elements.Count;
      }
    }

    ///<summary>
    ///Initialises a new vector with one element.
    ///</summary>
    ///<param name="elem">the element the vector contains</param>
    ///<param name="transposed">transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Vector(double elem, bool transposed = false)
    {
      _transposed = transposed;
      _elements.Add(elem);
    }

    ///<summary>
    ///Initialises a new vector with any amount of elements.
    ///</summary>
    ///<param name="elems">A list of elements to initialise the vector from</param>
    ///<param name="transposed">transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Vector(IEnumerable<double> elems, bool transposed = false)
    {
      _transposed = transposed;
      _elements.AddRange(elems);
    }

    ///<summary>
    ///Initialises a new null vector
    ///</summary>
    ///<param name="transposed">transposed state of the vector, this is optional
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
      _transposed = _transposed == false ? true : false;
    }

    public static Vector operator *(Vector vector, double scalar) {
      List<double> elems = new List<double>();
      foreach (v in vector)
        elems.Add(v * scalar);
      return new Vector(elems, vector.Transposed);
    }
/* old version
    public Vector Scale(double scalar)
    {
      return DotScalar(this, scalar);
    }
*/
    public static double operator *(Vector vector1, Vector vector2)
    {
      if (vector1.Dimensions != vector2.Dimensions)
        throw new IncompatibleDimensionsException("Vectors with different dimensions cannot be dotted.");

      double result = 0;
      for (int i = 0; i < vector1.Dimensions; i++)
      {
        result =+ vector1[i] * vector2[i];
      }
      return result;
    }
/* old version
    public double Dot(Vector vector)
    {
      return Dot(this, vector);
    }
*/
  }

  public class IncompatibleDimensionsException : Exception
  {
    ///<summary>
    ///construct exception
    ///</summary>
    public IncompatibleDimensionsException()
    {
    }

    ///<summary>
    ///construct exception with message
    ///</summary>
    ///<param name="message">exception message</param>
    public IncompatibleDimensionsException(string message) 
      : base(message)
    {
    }
    ///<summary>
    ///construct exception with message and inner exception
    ///</summary>
    ///<param name="message">exception message</param>
    ///<param name="inner">inner exception</param>
    public IncompatibleDimensionsException(string message, Exception inner)
      : base(message, inner)
  }
}
