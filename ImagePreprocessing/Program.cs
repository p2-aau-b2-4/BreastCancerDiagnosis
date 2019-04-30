using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Dicom;

namespace ImagePreprocessing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DdsmImage> ddsmImages =
                DdsmImage.GetAllImagesFromCsvFile(@"e:\brysttest\mass_case_description_train_set.csv");
            Parallel.ForEach(ddsmImages, ddsmImage =>
            { 
                var image = ddsmImage.DcomOriginalImage;
                Rectangle rectangle = Normalization.GetTumourPositionFromMask(ddsmImage.DcomMaskImage);
                image = Normalization.GetNormalizedImage(image,
                    rectangle, 500);
                image = Contrast.ApplyHistogramEqualization(image);
                image.SaveAsPng("images/ready" + Guid.NewGuid()+ ".png");
            });
        }


    }
}