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
using Microsoft.Win32;
using PCA;
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
            
            double[,] matrix = new double[,]
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
            
            SparseMatrix sM = SparseMatrix.OfArray(matrix);

            PCA.PCA pca = new PCA.PCA();
            
            pca.SolveEigenValues(sM);

        }
    }
}
