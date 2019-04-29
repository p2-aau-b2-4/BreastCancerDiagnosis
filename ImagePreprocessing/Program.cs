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
            foreach (var ddsmImage in ddsmImages)
            {
                var image = ddsmImage.DcomCroppedImage;
                image.SaveAsPng("muchOriginal.png");
                image = Normalization.ResizeImage(image, 100, 1000);
                image.SaveAsPng("muchResized.png");
                


                break;
            }
        }


    }
}