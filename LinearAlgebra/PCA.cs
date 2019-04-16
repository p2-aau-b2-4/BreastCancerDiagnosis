using System;
using System.Collections.Generic;
using System.Numerics;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
//using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PCA
{
    public class PCA
    {
        public PCA()
        {
        }

        private void MeanSubtraction(SparseMatrix matrix)
        {
            int tmp = matrix.RowCount;
            double sum = 0;
            double xI = 0;

            for (int x = 0; x < matrix.RowCount; x++)
            {
                for (int y = 0; y < matrix.ColumnCount; y++)
                {
                    Console.Write(matrix.Storage[x, y] + " ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("\n");

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                for (int x = 0; x < tmp; x++)
                {
                    sum += matrix.Storage[x, i];
                }

                xI = sum / tmp;
                sum = 0;

                for (int x = 0; x < tmp; x++)
                {
                    matrix.Storage[x, i] -= xI;
                }

                xI = 0;
            }

            for (int x = 0; x < matrix.RowCount; x++)
            {
                for (int y = 0; y < matrix.ColumnCount; y++)
                {
                    Console.Write(matrix.Storage[x, y] + " ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("\n");
        }

        private void CombinationsSubset(int numberOfElements, int numberOfDimensions, List<int> subsets)
        {
            CombinationsCovariance(0, 0, numberOfElements, numberOfDimensions, subsets);
        }

        private void CombinationsCovariance(int set, int indexPos, int r, int n, List<int> subsets)
        {
            if (r == 0)
            {
                subsets.Add(set);
            }
            else
            {
                for (int i = indexPos; i < n; i++)
                {
                    set = set | (1 << i);

                    CombinationsCovariance(set, i + 1, r - 1, n, subsets);

                    set = set & ~(1 << i);
                }
            }
        }

        private double Covariance(double x, double y, int dim)
        {
            return (x * y) / (dim - 1);
        }

        private SparseMatrix CovarianceMatrix(SparseMatrix sparseMatrix)
        {
            MeanSubtraction(sparseMatrix);

            double[,] cMatrix = new double[sparseMatrix.ColumnCount, sparseMatrix.ColumnCount];

            for (int x = 0; x < sparseMatrix.ColumnCount; x++)
            {
                for (int y = 0; y < sparseMatrix.ColumnCount; y++)
                {
                    for (int i = 0; i < sparseMatrix.RowCount; i++)
                    {
                        cMatrix[x, y] += Covariance(sparseMatrix.Storage[i, x], sparseMatrix.Storage[i, y],
                            sparseMatrix.RowCount);
                    }
                }
            }

            for (int y = 0; y < sparseMatrix.ColumnCount; y++)
            {
                for (int x = 0; x < sparseMatrix.ColumnCount; x++)
                {
                    Console.Write(Math.Round(cMatrix[x, y], 5) + " ");
                }

                Console.WriteLine();
            }

            return SparseMatrix.OfArray(cMatrix);
        }
        
        public void SolveEchelonForm(SparseMatrix sparseMatrix)
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

        private void SwapRows(SparseMatrix sparseMatrix, int i, int r)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> vector = sparseMatrix.Row(i);
            
            sparseMatrix.SetRow(i,sparseMatrix.Row(r));
            sparseMatrix.SetRow(r,vector);
        }

        private double Determinant(SparseMatrix sparseMatrix)
        {
            return sparseMatrix.Determinant();
        }
    }
}
