using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;

namespace DicomDisplayTest
{
    public class Program2
    {
        public static void RunPCA()
        {
            PixelVector[,] iv = new PixelVector[28,28];
            
            Bitmap bmap = new Bitmap("mnist3/0_5.png");
            
            double[,] dArray = new double[28,28];

            for (var img = 0; img < 1; img++)
            {
                for (var x = 0; x < bmap.Width; x++)
                {
                    for (var y = 0; y < bmap.Height; y++)
                    {
                        iv[x,y] = new PixelVector(x,y,bmap.GetPixel(x, y).R);
                        dArray[x, y] = bmap.GetPixel(x, y).R;
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
            // Create an Eigenvalue decomposition
            var evd = new EigenvalueDecomposition(M);
            // Store the eigenvalues and eigenvectors
            double[] λ = evd.RealEigenvalues;
            double[,] V = evd.Eigenvectors;
            // Reconstruct M = V*λ*V'
            double[,] R =
                V.MultiplyByDiagonal(λ).MultiplyByTranspose(V);
            // Show on screen
            Console.WriteLine("Matrix: ");
            Console.WriteLine(M.ToString(" 0"));
            Console.WriteLine();
            Console.WriteLine("Eigenvalues: ");
            Console.WriteLine(λ.ToString(" +0.000; -0.000;"));
            Console.WriteLine();
            Console.WriteLine("Eigenvectors:");
            Console.WriteLine(V.ToString(" +0.000; -0.000;"));
            Console.WriteLine();
            Console.WriteLine("Reconstruction:");
            Console.WriteLine(R.ToString(" 0"));

            int maxVal = 0;
            for (var x = 0; x < bmap.Width; x++)
            {
                for (var y = 0; y < bmap.Height; y++)
                {
                    if ((int) R[x, y] > maxVal)
                        maxVal = (int) R[x, y];
                }
            }
            
            Bitmap Rbmap = new Bitmap(28,28);
            for (var x = 0; x < bmap.Width; x++)
            {
                for (var y = 0; y < bmap.Height; y++)
                {
                    decimal res = ExtensionMethods.Map((int)R[x, y],0,255,0,maxVal);
                    if (res < 0)
                        res = 0;
                    if (res > 255)
                        res = 255;
                    Rbmap.SetPixel(x, y, Color.FromArgb((int)res,(int)res,(int)res));
                }
            }
            
            Rbmap.Save("Rtest.png",ImageFormat.Png);
            
            /*var pca = new PrincipalComponentAnalysis()
            {    
                Method = PrincipalComponentMethod.Center,
                Whiten = true
            };*/
            
            // Now we can learn the linear projection from the data
            //MultivariateLinearRegression transform = pca.Learn(dArray);
            
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