using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using Accord.Math.Decompositions;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Accord.Statistics;
using Accord.Statistics.Kernels;
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

        public double[] StandardDeviations { get; set; }


        public double[] SingularValues { get; set; }

        public double[] Eigenvalues { get; set; }


        public double[][] ComponentVectors { get; set; }

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
        public double[] GetComponentsFromImage(double[,] image, int numberOfDimensions)
        {
            if (ComponentVectors.Length <= 0)
            {
                Console.WriteLine("Run PCA train first");
                throw new Exception();
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

            double[] res = new double[numberOfDimensions];

            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < ComponentVectors[i].Length; j++)
                {
                    res[i] += matrix[j] * ComponentVectors[i][j];
                }
            }
            
            
            return res;
        }

        ///<summary>
        ///Trains a PCA model given a set of data
        ///</summary>
        ///<param name=images>a List of UShortArrayAsImage to perform training on</param>
        public void Train(List<UShortArrayAsImage> images)
        {
            List<UShortArrayAsImage> reduced =new List<UShortArrayAsImage>();
            int u = 0;
            foreach (var image in images)
            {
                u++;
                //if (u % 5 != 0) continue;
                reduced.Add(image);
                
            }

            images = reduced;//todo temporary
            
            
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
            SparseMatrix convertedToSparse = SparseMatrix.OfArray(data.ToMatrix());
            double[,] centeredData = MeanSubtraction(convertedToSparse).ToArray();
            double[,] covarianceMatrix = CovarianceMatrix(centeredData);
            SolveForEigen(covarianceMatrix);
            

            Console.WriteLine("PCA done");
        }

        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a SparseMatrix to perform MeanSubtraction on</param>
        public SparseMatrix MeanSubtraction(SparseMatrix matrix) //todo make native double
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
        
        public double[,] CovarianceMatrix(double[,] matrix)
        {
            double[,] scArrayMatrix = new double[matrix.Columns(), matrix.Columns()];
            double[,] tmpArrayMatrix = matrix;
            Parallel.For(0, matrix.Columns(),
                x =>
                {
                    for (int i = 0; i < matrix.Rows(); i++)
                    {
                        for (int y = 0; y < matrix.Columns(); y++)
                        {
                            scArrayMatrix[x, y] +=
                                (tmpArrayMatrix[i, x] * tmpArrayMatrix[i, y]) / (matrix.Rows() - 1);
                        }
                    }
                });
            return scArrayMatrix;
        }
        
        public void SolveForEigen(double[,] matrix)
        {
            if (matrix.Rows() != matrix.Columns())
                throw new ArgumentException("Must be quadratic");
            
            EigenvalueDecomposition evd = new EigenvalueDecomposition(matrix,true,true,true);

            ComponentVectors = evd.Eigenvectors.ToJagged();
            Eigenvalues = evd.RealEigenvalues;
        }
    }
}