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
    private List<double> _A;

    ///<summary>
    ///Stores the cumulative number of non-zero elements up to (not including)
    ///the ith row. Defined by the recursive relation:
    ///IA[0] = 0
    ///IA[i] = IA[i-1] + num of non-zero elements in the (i-1)th row.
    ///</summary>
    private List<double> _IA;

    ///<summary>
    ///Stores the column index of each element in _A
    ///</summary>
    private List<double> _JA;

    ///<summary>
    ///Nonzero elements in the matrix.
    ///</summary>
    private int NNZ;

    ///<summary>
    ///internal field containing the transposed state of the Matrix.
    ///</summary>
    private bool _transposed;

    public bool Transposed { get; }

    public int? Dimensions
    {
      get
      {
        return null;
      }
    }

    public Matrix(double[,] matrix, bool transposed = false)
    {
      _transposed = transposed;
      _n = matrix.GetLength(0);
      _m = matrix.GetLength(1);
      _IA = {0};

      for (int i = 0; i < _m; i++)
      {
        for (int j = 0; j < _n; j++)
        {
          if (matrix[i, j] != 0)
          {
            _A.Add(matrix[i, j]);
            _JA.Add(j);

            _NNZ++;
          }
        }
        _IA.Add(NNZ);
      }
    }

    ///<summary>
    ///Initialises a new vector with any amount of elements.
    ///</summary>
    ///<param name="elems">A list of elements to initialise the vector from</param>
    ///<param name="transposed">transposed state of the vector, this is optional
    ///and defaults to false</param>
    public Matrix(List<Vector> vectors, bool transposed = false)
    {
      _transposed = transposed;
      _n = matrix.GetLength(0);
      _m = matrix.GetLength(1);
      _IA = {0};

      for (int i = 0; i < _m; i++)
      {
        for (int j = 0; j < _n; j++)
        {
          if (matrix[i, j] != 0)
          {
            _A.Add(matrix[i, j]);
            _JA.Add(j);

            _NNZ++;
          }
        }
        _IA.Add(NNZ);
      }
    }
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

    public Vector operator *(Vector vector, double scalar) {
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
    public double operator *(Vector vector1, Vector vector2)
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
}
