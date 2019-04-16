using System;
using System.Collections.Generic;
using System.Numerics;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DimensionReduction
{
    public class PCA
    {
        public PCA()
        {
        }
        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a SparseMatrix to perform MeanSubtraction on</param>
        public void MeanSubtraction(SparseMatrix matrix)
        {
            double sum = 0;
            double xI = 0;

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                for (int x = 0; x < matrix.RowCount; x++)
                {
                    sum += matrix.Storage[x, i];
                }

                if (Double.IsNegativeInfinity(sum) || Double.IsInfinity(sum))
                  throw new NotFiniteNumberException(sum);
                xI = sum / matrix.RowCount;
                sum = 0;

                for (int x = 0; x < matrix.RowCount; x++)
                {
                    matrix.Storage[x, i] -= xI;
                }

                xI = 0;
            }
        }
        
        private double Covariance(double x, double y, int dim)
        {
            return (x * y) / (dim - 1);
        }

        public SparseMatrix CovarianceMatrix(SparseMatrix sparseMatrix)
        {
            MeanSubtraction(sparseMatrix);

            double[,] cMatrix = new double[sparseMatrix.ColumnCount, sparseMatrix.ColumnCount];

            for (int x = 0; x < sparseMatrix.ColumnCount; x++)
            {
                for (int y = 0; y < sparseMatrix.ColumnCount; y++)
                {
                    for (int i = 0; i < sparseMatrix.RowCount; i++)
                    {
                        cMatrix[x, y] = Covariance(sparseMatrix.Storage[i, x], sparseMatrix.Storage[i, y],
                            sparseMatrix.RowCount);
                    }
                }
            }

            return SparseMatrix.OfArray(cMatrix);
        }
        
        public void SolveEigenValues(SparseMatrix sparseMatrix)
        {
            sparseMatrix = CovarianceMatrix(sparseMatrix);
            
            var evd = sparseMatrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = sparseMatrix.Evd();
            Console.WriteLine("Her kommer the d");
            Console.WriteLine(eigen.EigenValues);
            Console.WriteLine(eigen.EigenVectors);
            Console.WriteLine(evd.EigenValues);
            Console.WriteLine(evd.EigenVectors);
        }

        public void SolveEigenVectors(SparseMatrix sparseMatrix)
        {
            
        }
    }
}
