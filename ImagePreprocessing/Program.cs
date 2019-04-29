using System;
using System.Collections.Generic;
using System.Configuration;
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
                var image = ddsmImage.DcomCroppedImage;
                image = Normalization.GetNormalizedImage(image,
                    Normalization.GetTumourPositionFromMask(ddsmImage.DcomMaskImage), 500);
                image.SaveAsPng("images/ready"+i+".png");

                i++;
            }
        }


    }
}