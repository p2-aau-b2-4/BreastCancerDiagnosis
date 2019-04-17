using System;
using System.Collections.Generic;
using System.Numerics;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using ImagePreprocessing;


namespace DimensionReduction
{
    public class PCA
    {
        public PCA()
        {
        }

        public void LoadModelFromFile(string path)
        {
            //Load database
        }

        public void SaveModelToFile(string path)
        {
            //Save to database
        }

        public void GetComponentsFromImage(int count, int image)
        {
            //TBD option 1: weighted solution. option 2: other solution.
        }

        public void Train(List<UshortArrayAsImage> images)
        {
            double[,] allImages = new double[images[0].PixelCount, images.Count];
            int i = 0;
            foreach (var image in images)
            {
                double[] dImage = new double[image.PixelCount];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        dImage[y * image.Height + x] = image.PixelArray[x,y];
                    }
                }

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        allImages[i, x] = dImage[y * image.Width + x];
                    }
                }
                
                i++;
            }
            
            SparseMatrix matrix = SparseMatrix.OfArray(allImages);
                
            MeanSubtraction(matrix);
            matrix = CovarianceMatrix(matrix);
            SolveEigenValues(matrix);
            SolveEigenVectors(matrix);
        }

        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a SparseMatrix to perform MeanSubtraction on</param>
        public void MeanSubtraction(SparseMatrix matrix)
        {
            var sums = matrix.ColumnSums();
            int index = 0;
            foreach (var sum in sums)
            {
                if (Double.IsNegativeInfinity(sum) || Double.IsInfinity(sum))
                  throw new NotFiniteNumberException(sum);
                double xI = sum / matrix.RowCount;
                
                matrix.SetColumn(index, matrix.Column(index).Subtract(xI));

                index++;
            }
        }
        
        ///<summary>
        ///Finds covariance for two doubles
        ///</summary>
        ///<param name=x>first value</param>
        ///<param name=y>second value</param>
        ///<param name=dim>dimensions of parent matrix</param>
        private double Covariance(double x, double y, int dim)
        {
            return (x * y) / (dim - 1);
        }

        ///<summary>
        ///Finds the covariance matrix of a SparseMatrix
        ///</summary>
        ///<param name=matrix>input matrix</param>
        public SparseMatrix CovarianceMatrix(SparseMatrix matrix)
        {
            double[,] cMatrix = new double[matrix.ColumnCount, matrix.ColumnCount];

            for (int x = 0; x < matrix.ColumnCount; x++)
            {
                for (int y = 0; y < matrix.ColumnCount; y++)
                {
                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        cMatrix[x, y] += Covariance(matrix.Storage[i, x], matrix.Storage[i, y],
                            matrix.RowCount);
                    }
                }
            }

            return SparseMatrix.OfArray(cMatrix);
        }
        
        ///<summary>
        ///Finds the eigen values of a matrix.
        ///</summary>
        ///<param name=matrix>input matrix. Must be square</param>
        public void SolveEigenValues(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
              throw new ArgumentException();

            var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenValues");
            Console.WriteLine(eigen.EigenValues);
            Console.WriteLine(evd.EigenValues);
        }

        public void SolveEigenVectors(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenVectors");
            Console.WriteLine(eigen.EigenVectors);
            Console.WriteLine(evd.EigenVectors);
        }
    }
}
