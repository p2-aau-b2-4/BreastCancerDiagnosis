using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using Dicom;

namespace ImagePreprocessing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DdsmImage> ddsmImages =
                DdsmImage.GetAllImagesFromCsvFile(@"e:\brysttest\mass_case_description_train_set.csv");
            int i = 0;
            foreach (var ddsmImage in ddsmImages)
            {
                if (i < 79)
                {
                    i++; continue;}
                var image = ddsmImage.DcomOriginalImage;
                Rectangle rectangle = Normalization.GetTumourPositionFromMask(ddsmImage.DcomMaskImage);
                Console.WriteLine($"{i} = {rectangle.X},{rectangle.Y},{rectangle.Width},{rectangle.Height}");
                Console.WriteLine($"{ddsmImage.DcomMaskFilePath}");
                image = Normalization.GetNormalizedImage(image,
                    rectangle, 500);
                image = Contrast.ApplyHistogramEqualization(image);
                image.SaveAsPng("images/ready"+i+".png");
                i++;
            }
        }


    }
}