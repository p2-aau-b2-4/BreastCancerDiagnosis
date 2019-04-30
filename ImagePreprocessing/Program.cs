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
                DdsmImage.GetAllImagesFromCsvFile(ConfigurationManager.AppSettings["trainingSetCsvPath"]);
            foreach (var x in ddsmImages)
            {
               // x.DcomOriginalImage.SaveAsPng("testest.png");
                
                break;
            }
        }
    }
}
