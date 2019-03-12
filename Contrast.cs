using Dicom.Imaging;

namespace DicomDisplayTest
{
    public class Contrast
    {
        public static DicomImage equalization(DicomImage image)
        {
            var image2 = image;
            int cdf_min = 255;

            // Copying image to int array 
            int[] pixels = new int[image2.Height * image2.Width];

            foreach (var VARIABLE in pixels)
            {
                pixels[VARIABLE] = image2.RenderImage().Pixels[VARIABLE];
            }

            // Finding the minimum brightness 
            foreach (var VARIABLE in pixels)
            {
                if (pixels[VARIABLE] < cdf_min && pixels[VARIABLE] > 0)
                {
                    cdf_min = pixels[VARIABLE];
                }
            }
            
            // Change pixels array
            
            return image2;
        }
    }
}