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
           /* List<DdsmImage> ddsmImages =
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
            */

           var img = DicomFile.Open(@"000000.dcm").GetUshortImageInfo();
           
           img.SaveAsPng("OriginalImage.png");
           
           Rectangle meh = new Rectangle(1,1,1,1);
            
           ushort[,] testValue = new ushort[,] { { 1, 1, 0, 0 }, { 0, 1, 0, 1}, { 0, 1, 0, 1}, { 0, 1, 1, 0 } };
           UShortArrayAsImage testImage = new UShortArrayAsImage(testValue);
           Normalization.ResizeImage(testImage, 6);


        }


    }
}