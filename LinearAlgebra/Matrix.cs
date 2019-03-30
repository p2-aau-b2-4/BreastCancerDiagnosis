using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
    private int _NNZ;

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
      _A = new List<double>();
      _JA = new List<double>();
      _IA = new List<double> {0};

      for (int i = 0; i < _m; i++)
      {
        for (int j = 0; j < _n; j++)
        {
          if (matrix[i, j] != 0.0)
          {
            _A.Add(matrix[i, j]);
            _JA.Add(j);

            _NNZ++;
          }
        }
        _IA.Add(_NNZ);
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
            if (_IA[j+1] > _IA[j] && valIndex < _NNZ && _JA[valIndex] == i) {
              matrix += $"{_A[valIndex]} ";
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
            if (_IA[i+1] > _IA[i] && valIndex < _NNZ && _JA[valIndex] == j) {
              matrix += $"{_A[valIndex]} ";
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
    
    public double Covariance()
    {
      
      return 0.0;
    }

    public void MeanSubtraction(double[,] matrix)
    {
      int tmp = 4;

      double sum = 0;
      
      double[] x_i = new double[tmp];
      double[] y_i = new double[tmp];
      
      for (int y = 0; y < tmp; y++)
      {
        for (int x = 0; x < tmp; x++)
        {
          sum += matrix[x, y];
        }
        x_i[y] = sum / tmp;
        sum = 0;
      }
      
      for (int x = 0; x < tmp; x++)
      {
        for (int y = 0; y < tmp; y++)
        {
          sum += matrix[x, y];
        }
        y_i[x] = sum / tmp;
        sum = 0;
      }
      
      
      
    }

    public void CombinationsCovariance(int set,
      int index_pos, int r, int n, dyn_array_int *subsets)
    {
      if (r == 0) {
        add_int_to_end_i(subsets, set);
      }
      else {
        for (int i = index_pos; i < n; i++) {
          set = set | (1 << i);

          CombinationsCovariance(set, i + 1, r - 1, n, subsets);

          set = set & ~(1 << i);
        }
      }
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