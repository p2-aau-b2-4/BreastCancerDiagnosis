using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using Dicom;
using Dicom.Imaging;
using DimensionReduction;
using Microsoft.Win32;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;


namespace DicomDisplayTest    
{
    class Program
    {
        static void Main(string[] args) {
            double[,] arr = new double[4,4] {
              {2, 0, 0, 3},
              {0, 0, 0, 0},
              {0, 5, 7, 0},
              {0, 4, 0, 0}
            };
            LinearAlgebra.Matrix m = new LinearAlgebra.Matrix(arr);
            SparseMatrix ms = SparseMatrix.OfArray(arr);

            Console.WriteLine(m.ToString());
            
            double[,] matrix1 = new double[,]
            {
                {2.5,2.4},
                {0.5,0.7},
                {2.2,2.9},
                {1.9,2.2},
                {3.1,3.0},
                {2.3,2.7},
                {2.0,1.6},
                {1.0,1.1},
                {1.5,1.6},
                {1.1,0.9}
            };
            
            double[,] matrix2 = new double[,]
            {
                {7,3},
                {3,-1}
            };
            
            double[,] matrix3 = new double[,]
            {
                {64,580,29},
                {66,570,33},
                {68,590,37},
                {69,660,46},
                {73,600,55}
            };
            
            double[,] matrix5 = new double[,]
            {
                
                {1,2,3,4,5,6},
                {1,2,3,4,5,6}
            };
            
            SparseMatrix sM1 = SparseMatrix.OfArray(matrix1);
            SparseMatrix sM2 = SparseMatrix.OfArray(matrix2);
            SparseMatrix sM3 = SparseMatrix.OfArray(matrix3);
            SparseMatrix sM4 = SparseMatrix.OfArray(arr);
            SparseMatrix sM5 = SparseMatrix.OfArray(matrix5);
            
            PCA pca = new PCA();

            pca.CovarianceMatrix(sM3);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(sM3[i,j].ToString());
                }

                Console.WriteLine();
            }

            sM1 = pca.CovarianceMatrix(sM1);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.Write(sM1[i,j].ToString());
                }

                Console.WriteLine();
            }
            
            pca.MeanSubtraction(sM5);
            sM5 = pca.CovarianceMatrix(sM5);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.Write(sM5[i,j].ToString());
                }

                Console.WriteLine();
            }

            //Console.WriteLine(sM3.Rank());

            /*pca.SolveEigenValues(sM1);
            pca.SolveEigenValues(sM2);
            pca.SolveEigenValues(sM4);*/
        }
    }
}
