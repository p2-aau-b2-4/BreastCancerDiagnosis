using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression.Linear;

namespace DicomDisplayTest
{
    public class Program2
    {
        public static void RunPCA()
        {
            PixelVector[,] iv = new PixelVector[28,28];
            
            double[][] dArray = new double[3370][];

            for (int i = 0; i < 3370; i++)
            {
                dArray[i] = new double[28*28];
            }

            for (var img = 0; img < 3370; img++)
            {
                string path = "mnist1/" + img + "_.png";
                Bitmap bmap = new Bitmap(path);
                for (var y = 0; y < 28; y++)
                {
                    for (var x = 0; x < 28; x++)
                    {
                        //iv[x,y] = new PixelVector(x,y,bmap.GetPixel(x, y).R);
                        dArray[img][(y*28)+x] = (double)bmap.GetPixel(x, y).R;
                        //int r = bmap.GetPixel(x, y).R;
                    }
                }
            }


            // Consider the following matrix
            double[,] M =
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 0, 34, 200, 74, 245, 153, 32, 70, 20, 5, 0, 0, 15, 122, 2, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 25, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 10, 70, 20, 5, 0, 0, 15, 12, 245, 55, 255, 10, 24, 52, 8 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 5, 153, 100, 70, 20, 5, 255, 10, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 },
                { 3, 2, 4, 128, 43, 65, 1, 255, 34, 200, 74, 245, 153, 100, 70, 20, 5, 0, 0, 15, 122, 245, 255, 255, 10, 254, 52, 28 }
            };
            
            /*double[][] pcaArray = new double[28*28][];
            for (int z = 0; z < 33; z++)
            {
                for (var y = 0; y < 28; y++)
                {
                    for (var x = 0; x < 28; x++)
                    {
                        pcaArray[(y * 28) + x] = dArray[z][(y * 28) + x];
                    }
                }
            }*/
            
            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis();
            var pissaf = pca.Learn(dArray);
            

            var pcaRevert = pca.Revert(pissaf.Transform(dArray));
            
            // Create an Eigenvalue decomposition
            //var evd = new EigenvalueDecomposition(M);
            // Store the eigenvalues and eigenvectors
            //double[] λ = evd.RealEigenvalues;
            //double[,] V = evd.Eigenvectors;
            // Reconstruct M = V*λ*V'
            /*double[,] R =
                V.MultiplyByDiagonal(λ).MultiplyByTranspose(V);*/
            //double[,] R =
              //  V.DotWithDiagonal(λ).DotWithTransposed(V);
            // Show on screen
            /*Console.WriteLine("Matrix: ");
            Console.WriteLine(dArray.ToString(" 0"));
            Console.WriteLine();
            Console.WriteLine("Eigenvalues: ");
            Console.WriteLine(λ.ToString(" +0.000; -0.000;"));
            Console.WriteLine();
            Console.WriteLine("Eigenvectors:");
            Console.WriteLine(V.ToString(" +0.000; -0.000;"));
            Console.WriteLine();
            Console.WriteLine("Reconstruction:");
            Console.WriteLine(R.ToString(" 0"));*/
            var piss = pissaf.Weights;
            double maxVal = 0;
            for (var y = 0; y < 28; y++)
            {
                for (var x = 0; x < 28; x++)
                {
                    if (Math.Abs(piss[0][(y*28)+x]) > maxVal)
                        maxVal = Math.Abs(piss[0][(y*28)+x]);
                }
            }
            
            Bitmap Rbmap = new Bitmap(28,28);
            for (int img = 0; img < 1; img++)
            {
                for (var y = 0; y < 28; y++)
                {
                    for (var x = 0; x < 28; x++)
                    {
                        //decimal res = ExtensionMethods.Map((int)R[x, y],0,255,minVal,maxVal);
                        
                        double res = Math.Abs(piss[img][(y * 28) + x]);
                        /*if (res != 0)
                            res = 255;*/
                        res = Map(res,0,maxVal,0,255);
                        Rbmap.SetPixel(x, y, Color.FromArgb((int)res,(int)res,(int)res));
                        pcaRevert[img][(y *28) + x] = res;
                    }
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("Reconstruction RGB:");
            //Console.WriteLine(pcaRevert.ToString(" 0"));
            Console.WriteLine(pissaf.Intercepts.ToString(" 0")); 
            
            
            Rbmap.Save("Rtest.png", ImageFormat.Png);
            
            /*var pca = new PrincipalComponentAnalysis()
            {    
                Method = PrincipalComponentMethod.Center,
                Whiten = true
            };*/
            
            // Now we can learn the linear projection from the data
            //MultivariateLinearRegression transform = pca.Learn(dArray);
            
        }
        
        protected static double Map(double s, double a1, double a2, double b1, double b2)
            // lånt fra https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }

    public class PixelVector
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int intensity { get; set; }
        
        public PixelVector(int x, int y, int intensity_in)
        {
            xPos = x;
            yPos = y;
            intensity = intensity_in;
        }

        public void SetPos(int x, int y)
        {
            xPos = x;
            yPos = y;
        }
    }
    
    public static class ExtensionMethods
    {
        public static decimal Map (this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}