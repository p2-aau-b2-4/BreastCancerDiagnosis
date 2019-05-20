using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using Dicom;
using Dicom.Imaging;
using DimensionReduction;
using ImagePreprocessing;
using Serializer = Accord.IO.Serializer;
//using Extreme.Mathematics;
using Extreme.Statistics;
//using Extreme.Statistics.Multivariate;
using MathNet.Numerics;
using Matrix = Extreme.Mathematics.Matrix;
using Vector = Accord.Math.Vector;
using Accord.IO;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using MathNet.Numerics.LinearAlgebra.Double;


namespace DicomDisplayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PCA pca2 = new PCA();

            List<UShortArrayAsImage> imagesToTrainOn = new List<UShortArrayAsImage>();
            List<UShortArrayAsImage> imagesToTrainOnn = new List<UShortArrayAsImage>();
            imagesToTrainOn = Serializer.Load<List<UShortArrayAsImage>>("readyImagesTrain.bin");
            imagesToTrainOn.GetRange(0, 10);

            foreach (var VAR in imagesToTrainOn)
            {
                imagesToTrainOnn.Add(Normalization.ResizeImage(VAR, 30));
            }
            
            pca2.Train(imagesToTrainOnn.GetRange(0,10));


            PrincipalComponentAnalysis pca4 = new PrincipalComponentAnalysis(PrincipalComponentMethod.CovarianceMatrix);


            double[,] allImages = PCA.GetMatrixFromImage(imagesToTrainOnn.GetRange(0,10));
            var covar = allImages.Covariance();
            //pca4.Learn(covar.ToJagged());
            
        }
    }
}