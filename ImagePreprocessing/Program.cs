using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using Dicom;

namespace ImagePreprocessing
{
    class Program
    {
        static void Main(string[] args)
        {

            List<DdsmImage> ddsmImages =
                DdsmImage.GetAllImagesFromCsvFile(ConfigurationManager.AppSettings["testSetCsvPath"]);

            foreach (var x in ddsmImages)
            {
               x.DcomOriginalImage.GetPngAsMemoryStream();
                break;
            }



        }
    }
}

