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
using MNIST.IO;
using Accord.Statistics;
using Accord.MachineLearning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using LinearAlgebra;

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

            m.Transpose();
            Console.WriteLine(m.ToString());

        }
    }
}
