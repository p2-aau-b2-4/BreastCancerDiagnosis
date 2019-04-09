using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LinearAlgebra
{
    public class PCA
    {
        public PCA()
        {
        }

        public void MeanSubtraction(SparseMatrix matrix)
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

        public void CombinationsSubset(int numberOfElements, int numberOfDimensions, List<int> subsets)
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

        public double Covariance(double x, double y, int dim)
        {
            return (x * y) / (dim - 1);
        }

        public double[,] CovarianceMatrix(SparseMatrix sparseMatrix)
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

            return cMatrix;
        }

        public void ReduceRowEchelonForm(SparseMatrix sparseMatrix)
        {
            int lead = 0;
            int rowCount = sparseMatrix.RowCount;
            int columnCount = sparseMatrix.ColumnCount;

            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount <= lead)
                {
                    break;
                }

                int i = r;

                while (sparseMatrix[i,lead] == 0)
                {
                    i++;
                    if (rowCount == i)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            break;
                        }
                    }
                }

                SwapRows();
                if (sparseMatrix[r,lead] != 0)
                {
                    //If M[r, lead] is not 0 divide row r by M[r, lead]
                }

                for (; i >= 0 && i < rowCount; i++)
                {
                    if (i != r)
                    {
                        //Subtract M[i, lead] multiplied by row r from row i
                    }
                }

                lead++;
            }
        }

        public void SwapRows()
        {
            
        }

        public double Determinant(SparseMatrix sparseMatrix)
        {
            return sparseMatrix.Determinant();
        }

        public void Eigenvectors(SparseMatrix sparseMatrix)
        {
            List<double> list = new List<double>();
            list.Add(1);
            list.Add(0);
            
            List<double> list2 = new List<double>();
            list2.Add(0);
            list2.Add(1);
            
            SparseVector sV = SparseVector.OfEnumerable(list);
            SparseVector sV2 = SparseVector.OfEnumerable(list2);
            
            List<SparseVector> sVL = new List<SparseVector>();
            sVL.Add(sV);
            sVL.Add(sV2);
            
            /*SparseMatrix sMa = SparseMatrix.OfColumns(sVL);

            sparseMatrix * sMa;
            
            Determinant();*/
        }

        public void UnitEigenvectors(double[,] cArr)
        {
            double len = 0.0, len2 = 0.0;

            len = Math.Sqrt((cArr[0, 0] * cArr[0, 0]) + (cArr[1, 0] * cArr[1, 0]));
            len2 = Math.Sqrt((cArr[0, 1] * cArr[0, 1]) + (cArr[1, 1] * cArr[1, 1]));

            cArr[0, 0] = cArr[0, 0] / len;
            cArr[1, 0] = cArr[1, 0] / len;

            cArr[0, 1] = cArr[0, 1] / len2;
            cArr[1, 1] = cArr[1, 1] / len2;

            Console.WriteLine();
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    Console.Write(Math.Round(cArr[x, y], 5) + " ");
                }

                Console.WriteLine();
            }
        }
    }
}