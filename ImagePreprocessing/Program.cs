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
                DdsmImage.GetAllImagesFromCsvFile(Configuration.Get("trainingSetCsvPath"));
            Console.WriteLine(ddsmImages.Count);
            /*
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

            /*var imgInfo= DicomFile.Open(@"000001.dcm").GetUByteImageInfo();
            imgInfo.SaveAsPng("eINFO.PNG");
            *///Normalization.ResizeImage(imgInfo, 6);
        }


    }
}
