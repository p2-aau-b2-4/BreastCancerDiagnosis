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

            Console.WriteLine(m.ToString());
            
            List<double> list = new List<double>();
            list.Add(2.5);
            list.Add(0.5);
            list.Add(2.2);
            list.Add(1.9);
            list.Add(3.1);
            list.Add(2.3);
            list.Add(2.0);
            list.Add(1.0);
            list.Add(1.5);
            list.Add(1.1);
            
            List<double> list2 = new List<double>();
            list2.Add(2.4);
            list2.Add(0.7);
            list2.Add(2.9);
            list2.Add(2.2);
            list2.Add(3.0);
            list2.Add(2.7);
            list2.Add(1.6);
            list2.Add(1.1);
            list2.Add(1.6);
            list2.Add(0.9);
            
            /*List<double> list3 = new List<double>();
            list3.Add(8);
            list3.Add(9);
            list3.Add(0);*/
            
            SparseVector sV = SparseVector.OfEnumerable(list);
            SparseVector sV2 = SparseVector.OfEnumerable(list2);
            //SparseVector sV3 = SparseVector.OfEnumerable(list3);
            
            List<SparseVector> sVL = new List<SparseVector>();
            sVL.Add(sV);
            sVL.Add(sV2);
            //sVL.Add(sV3);
            
            SparseMatrix sM = SparseMatrix.OfColumns(sVL);
            //m.Transpose();
            //m.MeanSubtraction(arr);
            //m.CombinationsSubset(2,3, list);
            PCA.PCA pca = new PCA.PCA();
            //double[,] cArr = pca.CovarianceMatrix(sM);
            //pca.UnitEigenvectors(cArr);
            pca.SolveEchelonForm();

            /*Console.WriteLine(list.Count);
            Console.WriteLine(list[0]);
            Console.WriteLine(list[1]);
            Console.WriteLine(list[2]);*/
            //Console.WriteLine(list[3]);
            //Console.WriteLine(m.ToString());

        }
    }
}
