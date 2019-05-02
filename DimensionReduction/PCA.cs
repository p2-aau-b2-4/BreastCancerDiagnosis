using System;
using System.Collections.Concurrent;
using Complex = System.Numerics.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Win32.SafeHandles;
using Vector = MathNet.Numerics.LinearAlgebra.Double.Vector;
using Accord.Statistics;


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
            //Select 1
        }

        public void Train(List<UShortArrayAsImage> images)
        {
            double[,] allImages = new double[images.Count, images[0].Width * images[0].Height];
            int i = 0;
            foreach (var image in images)
            {
                double[,] tempI = new double[image.Width, image.Height];
                Array.Copy(image.PixelArray, tempI, image.PixelArray.Length);
                double[] dImage = new double[image.Width * image.Height];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        //dImage[y * image.Width + x] = image.GetPixel(x,y).R;
                        dImage[y * image.Width + x] = tempI[x, y];
                    }
                }

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        allImages[i, y * image.Width + x] = dImage[y * image.Width + x];
                    }
                }

                i++;
                Console.WriteLine($"Copied image #{i}");
            }

            SparseMatrix matrix = SparseMatrix.OfArray(allImages);

            Console.WriteLine("done creating array");
            matrix = MeanSubtraction(matrix);

            Console.WriteLine("done meansubtraction");
            matrix = CovarianceMatrix(matrix);

            Console.WriteLine($"Done covariance");

            Evd<double> eigen = SolveForEigen(matrix);

            Console.WriteLine("done creating eigen");

            List<(Complex, Matrix<double>)> eigenLumps = new List<(Complex, Matrix<double>)>();

            List<List<double>> features = new List<List<double>>();
            List<System.Numerics.Vector<double>> meanSums = new List<System.Numerics.Vector<double>>();

            //Model model = new Model(eigen.EigenValues, eigen.EigenVectors, eigenLumps, features, new List<Vector<double>>());
            //model.SaveModelToFile("t.xml");
        }

        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a SparseMatrix to perform MeanSubtraction on</param>
        public SparseMatrix MeanSubtraction(SparseMatrix matrix)
        {
            var sums = matrix.ColumnSums();
            int index = 0;
            SparseMatrix tmpMatrix = new SparseMatrix(matrix.RowCount, matrix.ColumnCount);
            List<MathNet.Numerics.LinearAlgebra.Vector<double>> vectors =
                new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();
            matrix.CopyTo(tmpMatrix);
            foreach (var sum in sums)
            {
                if (Double.IsNegativeInfinity(sum) || Double.IsInfinity(sum))
                    throw new NotFiniteNumberException(sum);

                double xI = sum / matrix.RowCount;

                var tmpVector = matrix.Column(index);
                tmpVector = tmpVector.Subtract(xI);
                vectors.Add(tmpVector);

                index += 1;
            }

            SparseMatrix sMatrix = SparseMatrix.OfColumnVectors(vectors);
            return sMatrix;
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
        /* public SparseMatrix CovarianceMatrix(SparseMatrix matrix)
         {
             
             ConcurrentQueue<double[,]> count = new ConcurrentQueue<double[,]>();
             double[,] tmpArrayMatrix = matrix.ToArray();
             var cancellationTokenSource = new CancellationTokenSource();
             var addMatricesInQueue = new Task(() => AddMatricesInQueue(count,matrix.ColumnCount,matrix.RowCount,cancellationToken));
             Parallel.For(0, matrix.RowCount, (i, state) =>
             {
                 double[,] scArrayMatrix = new double[matrix.ColumnCount, matrix.ColumnCount];
                 Console.WriteLine($"{count.Count} / {matrix.RowCount}");
                 for (int x = 0; x < matrix.ColumnCount; x++)
                 {
                     for (int y = 0; y < matrix.ColumnCount; y++)
                     {
                         scArrayMatrix[x, y] += (tmpArrayMatrix[i, x] * tmpArrayMatrix[i, y]) / (matrix.RowCount - 1); // this is not threadsafe atm
                     }
                 }
                 count.Enqueue(scArrayMatrix);
             });
             cancellationTokenSource.Cancel();
 
             return await AddMatricesInQueue;
         }*/
        public SparseMatrix CovarianceMatrix(SparseMatrix matrix)
        {
            double[,] tmpArrayMatrix = matrix.ToArray();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var covMatrix = tmpArrayMatrix.Covariance();
            stopwatch.Stop();
            Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds}");
            return SparseMatrix.OfArray(covMatrix);
        }

        private async Task<double[,]> AddMatricesInQueue(ConcurrentQueue<double[,]> queue, int columns, int rows,
            CancellationToken cancellationToken)
        {
            double[,] scArrayMatrix = new double[columns, rows];
            while (queue.TryDequeue(out var toAdd) || !cancellationToken.IsCancellationRequested)
            {
                for (int x = 0; x < columns; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        scArrayMatrix[x, y] += toAdd[x, y];
                    }
                }
            }

            return scArrayMatrix;
        }

        ///<summary>
        ///Finds the eigen values and vectors of a matrix.
        ///</summary>
        ///<param name=matrix>input matrix. Must be square</param>
        public Evd<double> SolveForEigen(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            var eigen = matrix.Evd();

            /*List<Complex> eigenValues = new List<Complex>();
            for (int i = 0; i < evd.EigenValues.Count; i++)
            {
                model.EigenValues.Add(eigenValue);
            }

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                model.EigenVectors.Add(eigen.EigenVectors.Column(i));
            }*/

            return eigen;
        }
    }
}