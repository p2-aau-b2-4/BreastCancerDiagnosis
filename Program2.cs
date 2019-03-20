using System;
using System.Drawing;
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
            
            double[,][] dArray = new double[28,28][];

            for (var img = 0; img < 1; img++)
            {
                for (var x = 0; x < bmap.Width; x++)
                {
                    for (var y = 0; y < bmap.Height; y++)
                    {
                        iv[x,y] = new PixelVector(x,y,bmap.GetPixel(x, y).R);
                        
                    }
                }
            }
            
            var pca = new PrincipalComponentAnalysis()
            {    
                Method = PrincipalComponentMethod.Center,
                Whiten = true
            };
            
            // Now we can learn the linear projection from the data
            MultivariateLinearRegression transform = pca.Learn(dArray);


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

    public class ImageVector
    {
        
    }
}