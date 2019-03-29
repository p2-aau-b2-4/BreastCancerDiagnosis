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
    private List<double> val;

    ///<summary>
    ///Stores the cumulative number of non-zero elements up to (not including)
    ///the ith row. Defined by the recursive relation:
    ///IA[0] = 0
    ///IA[i] = IA[i-1] + num of non-zero elements in the (i-1)th row.
    ///</summary>
    private List<double> rowOffset;

    ///<summary>
    ///Stores the column index of each element in val
    ///</summary>
    private List<double> col;

    ///<summary>
    ///Nonzero elements in the matrix.
    ///</summary>
    private int nonZeroVals;

    ///<summary>
    ///internal field containing the transposed state of the Matrix.
    ///</summary>
    private bool _transposed;

    public bool Transposed
    {
      get
      {
        return _transposed; 
      }
    }

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
      val = new List<double>();
      col = new List<double>();
      rowOffset = new List<double> {0};

      for (int i = 0; i < _m; i++)
      {
        for (int j = 0; j < _n; j++)
        {
          if (matrix[i, j] != 0.0)
          {
            val.Add(matrix[i, j]);
            col.Add(j);

            nonZeroVals++;
          }
        }
        rowOffset.Add(nonZeroVals);
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
      
    }

    public override string ToString()
    {
      string matrix = new String("");
      int valIndex = 0;
      if (Transposed) {
        for (int i = 0; i < _n; i++) {
          for (int j = 0; j < _m; j++) {
            if (rowOffset[j+1] > rowOffset[j] && valIndex < nonZeroVals && col[valIndex] == i) {
              matrix += $"{val[valIndex]} ";
              valIndex++;
            }
            else {
              matrix += "0 ";
            }
          }
          matrix += "\n";
        }
      }
      else {
        for (int i = 0; i < _m; i++) {
          for (int j = 0; j < _n; j++) {
            if (rowOffset[i+1] > rowOffset[i] && valIndex < nonZeroVals && col[valIndex] == j) {
              matrix += $"{val[valIndex]} ";
              valIndex++;
            }
            else {
              matrix += "0 ";
            }
          }
          matrix += "\n";
        }
      }
      return matrix;
    }

    ///<summary> 
    ///Marks the vector as transposed
    ///</summary>
    public void Transpose()
    {
      _transposed = _transposed ? false : true;
    }
    
    /*public Matrix CovarianceMatrix()
    {
      
      return new Matrix();
    }

    public Matrix UnitEigenvectors()
    {
      return new Matrix();
    }*/
  }
}
