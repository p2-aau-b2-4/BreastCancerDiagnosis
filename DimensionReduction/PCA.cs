using System;
using System.Collections.Concurrent;
using Complex = System.Numerics.Complex;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Math.Distances;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Storage;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Win32.SafeHandles;
using Vector = MathNet.Numerics.LinearAlgebra.Double.Vector;
using Accord.Statistics;
using MathNet.Numerics.LinearAlgebra.Complex;
using SparseMatrix = MathNet.Numerics.LinearAlgebra.Double.SparseMatrix;


namespace DimensionReduction
{
    public class PCA
    {
        public Model model { get; set; }
        private double AverageMean { get; set; }
        private List<double> _eigenValues;
        private List<double[,]> _eigenVectors;
        private List<MathNet.Numerics.LinearAlgebra.Vector<double>> _meanSums = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();
        public PCA()
        {
            AverageMean = 0.0;
            _eigenValues = new List<double>();
            _eigenVectors = new List<double[,]>();
        }

        public void LoadModelFromFile(string path)
        {
            //Load database
        }

        public void SaveModelToFile(string path)
        {
            //Save to database
        }

        public void GetComponentsFromImage(SparseMatrix tmpmatrix, Vector<double> image, int numberOfComponents)
        {
            if (_eigenValues.Count <= 0)
            {
                Console.WriteLine("Run PCA train");
                return;
            }

            _eigenValues.Reverse();
            _eigenVectors.Reverse();

            SparseMatrix matrix = SparseMatrix.OfRowVectors(image);

            MeanSubtraction(matrix);
            
            

            Console.WriteLine(_eigenValues[0].ToString());
            Console.WriteLine(_eigenVectors[0].ToString());

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
            
            double[][] dArray = new double[images.Count][];
            i = 0;
            foreach (var image in images)
            {
                dArray[i] = new double[image.Width*image.Height];
                
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        //dImage[y * image.Width + x] = image.GetPixel(x,y).R;
                        dArray[i][y * image.Width + x] = image.PixelArray[x, y];
                    }
                }
                
                i += 1;
            }
            
            PrincipalComponentMethod1(dArray);

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
            //model = new Model(eigen.EigenValues, eigen.EigenVectors, eigenLumps, features, new List<Vector<double>>());
            foreach (var value in eigen.EigenValues)
            {
                _eigenValues.Add(value.Real);
            }

            /*int index = 0;
            for (int j = 0; j < eigen.EigenVectors.ColumnCount; j++)
            {
                foreach (var vector in eigen.EigenVectors.Column(j).Enumerate())
                {
                    _eigenVectors[index].Add(vector);
                    index += 1;
                }
            }*/

        }

        public void MeanFace(SparseMatrix matrix)
        {
            
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

                //model.MeanSums[index].Add(xI);
                AverageMean += xI;

                index += 1;
            }

            AverageMean /= index - 1;
            
            SparseMatrix sMatrix = SparseMatrix.OfColumnVectors(vectors);
            return sMatrix;
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double[,] scArrayMatrix = new double[matrix.ColumnCount, matrix.ColumnCount];
            double[,] tmpArrayMatrix = matrix.ToArray();

            
            Parallel.For(0, matrix.ColumnCount,
                x =>
                {
                    
                    // the middle for loop is parallized. The first cannot, since it would not be atomic, the third is not parallized, due to overhead. (was 68% faster without)

                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        for (int y = 0; y < matrix.ColumnCount; y++)

                        {
                            scArrayMatrix[x, y] +=
                                (tmpArrayMatrix[i, x] * tmpArrayMatrix[i, y]) / (matrix.RowCount - 1);
                        }
                    }

                    ;
                });


            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            return SparseMatrix.OfArray(scArrayMatrix);
        }


        /*public SparseMatrix CovarianceMatrix(SparseMatrix matrix)
        {
            double[,] tmpArrayMatrix = matrix.ToArray();
            int threads = Environment.ProcessorCount;
            
            Task<double[,]>[] tasks = new Task<double[,]>[threads];
            int rangeStart = 0;
            for (int i = 0; i < threads; i++)
            {
                // lets start as many tasks as threads computing part of covariance
                
                // we have to run this matrix.RowCount amount of times, lets split this between the different threads.
                int rangeSize = matrix.RowCount / threads;
                // this leaves up to Environt.ProcessorCount-1, lets find this count by modulos
                int left = matrix.RowCount % threads;
                if (i < left) rangeSize++; // lets add an extra range, to eveyr thread started, thats under the amount of extra range.
                int rangeStartThread = rangeStart.DeepClone();
                tasks[i] = Task.Factory.StartNew(() => GetCoCovarianceMatrix(tmpArrayMatrix,rangeStartThread,rangeSize), TaskCreationOptions.LongRunning);
                rangeStart += rangeSize;
            }
            Task.WaitAll(tasks);
            Console.WriteLine("done multithreading");            
            double[,] covMatrix  = new double[matrix.ColumnCount,matrix.ColumnCount];

            for (int i = 0; i < threads; i++)
            {
                double[,] result = tasks[i].Result;
                int xLength = result.GetLength(0);
                int yLength = result.GetLength(1);
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        covMatrix[x,y] += result[x, y];
                    }
                }
            }
            
            return SparseMatrix.OfArray(covMatrix);
        }*/

        /*private double[,] GetCoCovarianceMatrix(double[,] tmpArrayMatrix, int rangeStart, int rangeSize)
        {
            Console.WriteLine($"Got called with rs={rangeStart} size = {rangeSize}");
            double[,] scArrayMatrix = new double[tmpArrayMatrix.GetLength(1), tmpArrayMatrix.GetLength(1)];
            for (int i = rangeStart; i < rangeStart+rangeSize; i++)
            {
                int length = tmpArrayMatrix.GetLength(1);
                for (int x = 0; x < length; x++)
                {
                    for (int y = 0; y < length; y++)
                    {
                        scArrayMatrix[x, y] += (tmpArrayMatrix[i, x] * tmpArrayMatrix[i, y]) / (length - 1);
                    }
                }
            }
            return scArrayMatrix;
        }*/

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

        public void reverse()
        {
            
        }
        
        private double[] columnMeans;
        public double[] Means
        {
            get { return this.columnMeans; }
            set { this.columnMeans = value; }
        }
        
        private double[] columnStdDev;
        public double[] StandardDeviations
        {
            get { return this.columnStdDev; }
            set { this.columnStdDev = value; }
        }
        
        private double[] singularValues;
        public double[] SingularValues
        {
            get { return singularValues; }
            protected set { singularValues = value; }
        }
        
        private double[] eigenvalues;
        public double[] Eigenvalues
        {
            get { return eigenvalues; }
            protected set { eigenvalues = value; }
        }
        
        private double[][] eigenvectors;
        public double[][] ComponentVectors
        {
            get { return this.eigenvectors; }
            protected set { this.eigenvectors = value; }
        }
        
        public void PrincipalComponentMethod1(double[][] x)
        {
            this.Means = x.Mean(dimension: 0);

            //double[][] matrix = Overwrite ? x : Jagged.CreateAs(x);
            double[][] matrix = true ? x : Jagged.CreateAs(x);
            x.Subtract(Means, dimension: (VectorType)0, result: matrix);

            this.StandardDeviations = x.StandardDeviation(Means);
            matrix.Divide(StandardDeviations, dimension: (VectorType)0, result: matrix);

            //  The principal components of 'Source' are the eigenvectors of Cov(Source). Thus if we
            //  calculate the SVD of 'matrix' (which is Source standardized), the columns of matrix V
            //  (right side of SVD) will be the principal components of Source.

            // Perform the Singular Value Decomposition (SVD) of the matrix
            var svd = new JaggedSingularValueDecomposition(matrix,
                computeLeftSingularVectors: false,
                computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true);

            SingularValues = svd.Diagonal;
            Eigenvalues = SingularValues.Pow(2);
            Eigenvalues.Divide(x.Rows() - 1, result: Eigenvalues);
            ComponentVectors = svd.RightSingularVectors.Transpose();
            
        }
    }
}