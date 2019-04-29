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
                DdsmImage.GetAllImagesFromCsvFile(ConfigurationManager.AppSettings["testSetCsvPath"]);
            foreach (var x in ddsmImages)
            {
                x.DcomOriginalImage.SaveAsPng("testest.png");
                
                break;
            }
            //ddsmImages[14].GetNormalizedSizedCrop(100,100);
            //console.writeline($"found {ddsmimages.count}");
            //ddsmimages.first().getnormalizedsizedcrop(1000).saveaspng("black.png");

            //            Serializer.Save("data.bin", DDSMImages);
            //            foreach (DdsmImage ddsmImage in DDSMImages)
            //            {
            //                Console.WriteLine($"{ddsmImage.PatientId} | {ddsmImage.ImageView} | {ddsmImage.BreastSide} | {ddsmImage.GetDcomCroppedImage().Width}x{ddsmImage.GetDcomCroppedImage().Height}");
            //            }

           /* List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");

            var x = DDSMImages.First();
            x.GetDcomCroppedImage().PixelArray = x.GetDcomCroppedImage().PixelArray;
            x.GetNormalizedSizedCrop(1000).SaveAsPng("black.png");
            */


//            Serializer.Save("data.bin", DDSMImages);
//            foreach (DdsmImage ddsmImage in DDSMImages)
//            {
//                Console.WriteLine($"{ddsmImage.PatientId} | {ddsmImage.ImageView} | {ddsmImage.BreastSide} | {ddsmImage.GetDcomCroppedImage().Width}x{ddsmImage.GetDcomCroppedImage().Height}");
//            }

            // Lets render a picture:
            //            Console.WriteLine(DDSMImages[10].DcomMaskFilePath);

            //DDSMImages.First().GetDcomOriginalImage().SaveAsPng("original.png");
            //DDSMImages.First().GetDcomMaskImage().SaveAsPng("mask.png");
            //DDSMImages.First().GetDcomCroppedImage().SaveAsPng("cropped.png");

            //            var xImg = DDSMImages.First().GetDcomOriginalImage();
            //            var yImg = DDSMImages.First().GetDcomMaskImage();
            //
            //            Console.WriteLine(DDSMImages.First().Pathology);
            //            var bitmap = new Bitmap(yImg.Width, yImg.Height);
            //            for (int x = 0; x < yImg.Width; x++)
            //            {
            //                for (int y = 0; y < yImg.Height; y++)
            //                {
            //                    byte value = yImg.PixelArray[x, y];
            //                    bool difference = false;
            //                    for (int xin = x - 1; xin < x + 1; xin++)
            //                    {
            //                        if (xin < 0 || xin >= yImg.Width) continue;
            //                        for (int yin = y - 1; yin < y + 1; yin++)
            //                        {
            //                            if (yin < 0 || yin >= yImg.Height) continue;
            //                            if (yImg.PixelArray[xin, yin] != value) difference = true;
            //                        }
            //                    }
            //
            //                    Color color = Color.FromArgb(difference ? 255 : 0, 255, 0, 0);
            //                    bitmap.SetPixel(x, y, color);
            //                }
            //            }
            //
            //            xImg.AddOverlay(bitmap);
            //            xImg.SaveAsPng("test.png");

        }
    }
}
