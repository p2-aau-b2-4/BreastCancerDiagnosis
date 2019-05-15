using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using Accord.Math.Decompositions;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Accord.Statistics;
using SparseMatrix = MathNet.Numerics.LinearAlgebra.Double.SparseMatrix;

namespace DimensionReduction
{
    [Serializable]
    public class PCA
    {
        
        /// <summary>
        /// A private array containing all column means from the Training method 
        /// </summary>
        private double[] columnMeans;
        
        /// <summary>
        /// A properties that gets and sets columnMeans
        /// </summary>
      
        public double[] Means
        {
            get { return this.columnMeans; }
            set { this.columnMeans = value; }
        }
        
        /// <summary>
        /// A private array containing all column standard deviations
        /// </summary>
        private double[] columnStdDev;
        
        /// <summary>
        /// A properties that gets and sets columnStdDev
        /// </summary>
      
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

        public PCA()
        {
        }

        public static PCA LoadModelFromFile(string path)
        {
            return Serializer.Load<PCA>(path);
        }

        ///<summary>
        ///Projects a image onto a eigenspace that is made then the method Train is run
        ///</summary>
        ///<param name="image">a UShortArrayAsImage to perform projectionn with</param>
        ///<param name="numberOfComponents">Number of eigenvectors to project image onto</param>
        public double[] GetComponentsFromImage(UShortArrayAsImage image, int numberOfComponents)
        {
            double[,] tmpImage = new double[image.Width, image.Height];
            Array.Copy(image.PixelArray, tmpImage, image.PixelArray.Length);
            return GetComponentsFromImage(tmpImage, numberOfComponents);
        }

        ///<summary>
        ///Projects a image onto a eigenspace that is made then the method Train is run
        ///</summary>
        ///<param name="image">a 2D array to perform projectionn with</param>
        ///<param name="numberOfComponents">Number of eigenvectors to project image onto</param>
        public double[] GetComponentsFromImage(double[,] image, int numberOfComponents)
        {
            if (ComponentVectors.Length <= 0)
            {
                Console.WriteLine("Run PCA train first"); // todo throw exception
                return null;
            }

            int columns = image.Columns();
            int rows = image.Rows();

            double[] matrix = new double[image.Length];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i * columns + j] = image[i, j];
                }
            }

            SparseMatrix tmpMatrix = MeanSubtraction(matrix);

            for (int i = 0; i < rows; i++)
            {
                matrix[i] = tmpMatrix[0, i];
            }

            double[] res = new double[numberOfComponents];
            // multiply the data matrix by the selected eigenvectors
            // TODO: Use cache-friendly multiplication
            for (int j = 0; j < numberOfComponents; j++)
                for (int k = 0; k < ComponentVectors[j].Length; k++)
                    res[j] += matrix[k] * ComponentVectors[j][k];
            return res;
        }

        ///<summary>
        ///Trains a PCA model given a set of data
        ///</summary>
        ///<param name=images>a List of UShortArrayAsImage to perform training on</param>
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

            Train(allImages);
        }
        
        ///<summary>
        ///Trains a PCA model given a set of data
        ///</summary>
        ///<param name=data>a 2D array to perform training on</param>
        public void Train(double[,] data)
        {
            //data.ToJagged();
            int rows = data.Rows();
            int columns = data.Columns();

            double[][] dArray = new double[data.Rows()][];

            for (int j = 0; j < rows; j++)
            {
                dArray[j] = new double[columns];

                for (int x = 0; x < columns; x++)
                {
                    dArray[j][x] = data[j, x];
                }
            }

            Train(dArray);
        }

        ///<summary>
        ///Trains a PCA model given a set of data
        ///</summary>
        ///<param name=data>a Jagged array to perform training on</param>
        public void Train(double[][] data)
        {
            //data = data.Transpose();
            this.Means = data.Mean(dimension: 0);

            //double[][] matrix = Overwrite ? x : Jagged.CreateAs(x);
            double[][] matrix = true ? data : Jagged.CreateAs(data);
            data.Subtract(Means, dimension: (VectorType) 0, result: matrix);

            this.StandardDeviations = data.StandardDeviation(Means);
            matrix.Divide(StandardDeviations, dimension: (VectorType) 0, result: matrix);

            //  The principal components of 'Source' are the eigenvectors of Cov(Source). Thus if we
            //  calculate the SVD of 'matrix' (which is Source standardized), the columns of matrix V
            //  (right side of SVD) will be the principal components of Source.

            // Perform the Singular Value Decomposition (SVD) of the matrix
            Console.WriteLine("Perfoming SVD");
            var svd = new JaggedSingularValueDecomposition(matrix,
                computeLeftSingularVectors: false,
                computeRightSingularVectors: true,
                autoTranspose: false, inPlace: true);

            SingularValues = svd.Diagonal;
            Eigenvalues = SingularValues.Pow(2);
            Eigenvalues.Divide(data.Rows() - 1, result: Eigenvalues);
            ComponentVectors = svd.RightSingularVectors.Transpose();

            //Model model = new Model(eigen.EigenValues, eigen.EigenVectors, eigenLumps, features, new List<Vector<double>>());
            //model.SaveModelToFile("t.xml");
            //model = new Model(eigen.EigenValues, eigen.EigenVectors, eigenLumps, features, new List<Vector<double>>());

            Console.WriteLine("PCA done");
        }
        
        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a double array to perform MeanSubtraction on</param>
        public SparseMatrix MeanSubtraction(double[] matrix)
        {
            SparseMatrix test = new SparseMatrix(1, matrix.Length);

            for (int i = 0; i < matrix.Length; i++)
            {
                test[0, i] = matrix[i];
            }

            return MeanSubtraction(test);
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

                index += 1;
            }

            SparseMatrix sMatrix = SparseMatrix.OfColumnVectors(vectors);
            return sMatrix;
        }

        ///<summary>
        ///Finds the covariance matrix of a SparseMatrix
        ///</summary>
        ///<param name=matrix>input matrix</param>
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
                });


            sw.Stop();
            Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");
            return SparseMatrix.OfArray(scArrayMatrix);
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