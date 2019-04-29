using System;
using System.Collections.Generic;
using System.Drawing;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32.SafeHandles;
using Complex = System.Numerics.Complex;
using Vector = MathNet.Numerics.LinearAlgebra.Double.Vector;


namespace DimensionReduction
{
    public class PCA2
    {
        public PCA2()
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

        public void Train(List<Bitmap> images)
        {
            //double[,] allImages = new double[images[0].Width, images[0].Height/*images.Count*/];
            List<double[,]> allImages = new List<double[,]>();
            
            int i = 0;
            double sum = 0;
            foreach (var image in images)
            {
                //double[,] tempI = new double[image.Width,image.Height]; 
                //Array.Copy(image.PixelArray,tempI,image.PixelCount); 
                double[] dImage = new double[image.Width*image.Height];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        dImage[y * image.Width + x] = images[i].GetPixel(x,y).R;
                    }
                }

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        allImages[i][x, y] = dImage[y * image.Width + x];
                    }
                }

                i++;
            }
            Console.WriteLine("Images are loaded...");
            List<SparseMatrix> matrix = new List<SparseMatrix>();
            
            foreach (var image in allImages)
            {
                matrix.Add(SparseMatrix.OfArray(image));                
            }
            
            Console.WriteLine("Matrix created...");
            /*for (int yIndex = 0; yIndex < 28; yIndex++)
            {
                for (int xIndex = 0; xIndex < 28; xIndex++)
                {
                    if (allImages[xIndex,yIndex] > 0.0)
                        Console.Write(allImages[xIndex,yIndex]);
                }
                //Console.WriteLine();
            }*/
            matrix = MeanSubtraction(matrix);
            Console.WriteLine("Meansubtraction done...");
            matrix = CovarianceMatrix(matrix);
            Console.WriteLine("Covariance done...");
            SolveEigenValues(matrix);
            Console.WriteLine("EigenValues done...");
            SolveEigenVectors(matrix);
            Console.WriteLine("EigenVectors done...");
        }

        ///<summary>
        ///Finds the mean of each column in a matrix, then subtracts the mean of
        ///each column from all its values
        ///</summary>
        ///<param name=matrix>a SparseMatrix to perform MeanSubtraction on</param>
        public List<SparseMatrix> MeanSubtraction(List<SparseMatrix> allImages)
        {
            /*for (int xIndex = 0; xIndex < matrix.RowCount; xIndex++)
            {
                for (int yIndex = 0; yIndex < matrix.ColumnCount; yIndex++)
                {
                    Console.Write(matrix[xIndex,yIndex]);
                }
                Console.WriteLine();
            }*/

            foreach (var matrix in allImages)
            {
                var sums = matrix.ColumnSums();
                int index = 0;
                SparseMatrix tmpMatrix = new SparseMatrix(matrix.RowCount,matrix.ColumnCount);
                List<Vector<double>> vectors = new List<Vector<double>>();
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

                matrix = SparseMatrix.OfColumnVectors(vectors);
            }
            
            return allImages;
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
        public List<SparseMatrix> CovarianceMatrix(List<SparseMatrix> matrix)
        {
            double[,] cMatrix = new double[matrix[0].ColumnCount, matrix[0].ColumnCount];

            for (int x = 0; x < matrix[0].ColumnCount; x++)
            {
                for (int y = 0; y < matrix[0].ColumnCount; y++)
                {
                    for (int i = 0; i < matrix[0].RowCount; i++)
                    {
                        double val = Covariance(matrix.Storage[i, x], matrix.Storage[i, y],
                                matrix.RowCount);

                        if (Double.IsNegativeInfinity(val) || Double.IsInfinity(val))
                            throw new NotFiniteNumberException(val);

                        cMatrix[x, y] += val;
                    }
                }
            }

            /*for (int y = 0; y < matrix.ColumnCount; y++)
            {
                for (int x = 0; x < matrix.ColumnCount; x++)
                {
                    Console.WriteLine(cMatrix[x,y]);
                }
            }*/
            return SparseMatrix.OfArray(cMatrix);
        }

        ///<summary>
        ///Finds the eigen values of a matrix.
        ///</summary>
        ///<param name=matrix>input matrix. Must be square</param>
        public Vector<Complex> SolveEigenValues(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            //var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            Console.WriteLine("After asymmetric");
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenValues");
            //Console.WriteLine(eigen.EigenValues);
            //Console.WriteLine(evd.EigenValues);
            return eigen.EigenValues;
        }

        public Matrix<double> SolveEigenVectors(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            //var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenVectors");
            //Console.WriteLine(eigen.EigenVectors);
            //Console.WriteLine(evd.EigenVectors);
            return eigen.EigenVectors;
        }
    }
}
