using System;
using System.Collections.Generic;
using System.Drawing;
using BitMiracle.LibJpeg.Classic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using ImagePreprocessing;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Win32.SafeHandles;
using Vector = MathNet.Numerics.LinearAlgebra.Double.Vector;


namespace DimensionReduction
{
    public class PCA3
    {
        public PCA3()
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
            double[,] allImages = new double[images.Count, images[0].Width*images[0].Height];
            int i = 0;
            foreach (var image in images)
            {
                /*double[,] tempI = new double[image.Width,image.Height]; 
                Array.Copy(image.PixelArray,tempI,image.PixelCount);*/ 
                double[] dImage = new double[image.Width*image.Height];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        dImage[y * image.Width + x] = image.GetPixel(x,y).R;
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
            }

            Console.WriteLine("1");
            SparseMatrix matrix = SparseMatrix.OfArray(allImages);
            Console.WriteLine("2");
            matrix = MeanSubtraction(matrix);
            Console.WriteLine("3");
            matrix = CovarianceMatrix(matrix);
            Console.WriteLine("4");
            var evd = SolveEigenValues(matrix);
            Console.WriteLine("5");
            
            List<string> lines = new List<string>();

            foreach (var eigenValues in evd.EigenValues.Enumerate())
            {
                lines.Add(eigenValues.Real.ToString());
            }
            
            lines.Add("");

            /*for (int j = 0; j < matrix.RowCount; j++)
            {
                string tmp = "";
                for (int k = 0; k < matrix.ColumnCount; k++)
                {
                    tmp += " " + evd.EigenVectors.Row(j)[k];
                }
                lines.Add("");
                lines.Add(tmp);
            }*/
            
            for (int j = 0; j < matrix.ColumnCount; j++)
            {
                string tmp = "";
                for (int k = 0; k < matrix.RowCount; k++)
                {
                    tmp += " " + evd.EigenVectors.Column(j)[k];
                }
                lines.Add("");
                lines.Add(tmp);
            }
            
            using (System.IO.StreamWriter file = 
                new System.IO.StreamWriter(@"/home/yogi/Documents/Uni/2_semester/P2/github/BreastCancerDiagnosis/DimensionReduction/WriteLines_column.txt"))
            {
                foreach (string line in lines)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(line);
                }
            }

            double[,] newBitmap = new double[28,28];
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    newBitmap[x, y] = Math.Abs(evd.EigenVectors.Column(783)[y*28+x]);
                    newBitmap[x, y] *= 1000;
                    if (newBitmap[x, y] > 255)
                        newBitmap[x, y] = 255;
                    else if (newBitmap[x, y] < 0.0)
                        newBitmap[x, y] = 0;
                }
            }
            Bitmap b = new Bitmap(28,28);
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    int val = (int)newBitmap[x, y];
                    b.SetPixel(x, y, Color.FromArgb(val,val,val));
                }
            }
            b.Save("eigen1.png");
            
            
            /*SolveEigenVectors(matrix);
            Console.WriteLine("6");*/
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

            SparseMatrix sMatrix = SparseMatrix.OfColumnVectors(vectors);
            /*Model model = new Model(); Ved ikke lige hvordan model skal implementeres om den skal være statisk?
            foreach (var vectorSum in vectors)
            {
                model.MeanSums.Add(vectorSum);                
            }*/
            
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
        public SparseMatrix CovarianceMatrix(SparseMatrix matrix)
        {
            double[,] cMatrix = new double[matrix.ColumnCount, matrix.ColumnCount];
            SparseMatrix tmpMatrix = new SparseMatrix(matrix.RowCount,matrix.ColumnCount);
            matrix.CopyTo(tmpMatrix);

            for (int x = 0; x < matrix.ColumnCount; x++)
            {
                for (int y = 0; y < matrix.ColumnCount; y++)
                {
                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        double val = Covariance(tmpMatrix[i, x], tmpMatrix[i, y],
                                matrix.RowCount);

                        if (Double.IsNegativeInfinity(val) || Double.IsInfinity(val))
                            throw new NotFiniteNumberException(val);

                        cMatrix[x, y] += val;
                    }
                }
            }

            return SparseMatrix.OfArray(cMatrix);
        }

        ///<summary>
        ///Finds the eigen values of a matrix.
        ///</summary>
        ///<param name=matrix>input matrix. Must be square</param>
        public Evd<double> SolveEigenValues(SparseMatrix matrix) //Outdated use SolveForEigen
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            //var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenValues");
            Console.WriteLine(eigen.EigenValues);
            //Console.WriteLine(evd.EigenValues);
            return eigen;
        }

        public void SolveEigenVectors(SparseMatrix matrix) //Outdated use SolveForEigen
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            //var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenVectors");
            Console.WriteLine(eigen.EigenVectors);
            //Console.WriteLine(evd.EigenVectors);
        }
        
        public void SolveForEigen(SparseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException();

            //var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Asymmetric);
            var eigen = matrix.Evd();
            Console.WriteLine("Her kommer the EigenVectors");
            Console.WriteLine(eigen.EigenVectors);
            //Console.WriteLine(evd.EigenVectors);
            
            Model model = new Model();

            foreach (var eigenValue in eigen.EigenValues)
            {
                model.EigenValues.Add(eigenValue);
            }

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                model.EigenVectors.Add(eigen.EigenVectors.Column(i));
            }
        }
    }
}
