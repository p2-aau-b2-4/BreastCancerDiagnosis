using System;
using System.Configuration;
using Dicom;

namespace ImagePreprocessing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ConfigurationManager.AppSettings["myKey"]);
            var img = DicomFile.Open(@"000000.dcm");
            UshortArrayAsImage imgInfo = img.GetUshortImageInfo();
            imgInfo.SaveAsPng("eINFO.PNG");

            imgInfo.SaveAsPng("LarsLavedeDetHerFør.png");
            imgInfo.ApplyHistogramEqualization();
            imgInfo.SaveAsPng("LarsLavedeDetHerEfter.png");

            imgInfo.ApplyContrastEnhancement(100);

            
            imgInfo.SaveAsPng("eINFOContrast.png");    //REMOVE
            //var imgOverlay = imgInfo.Edge(1000);
            //imgOverlay.SaveAsPng("OliverErEnNar.png");
            
            //list<ddsmimage> ddsmimages =
            //    ddsmimage.getallimagesfromcsvfile(@"e:\brysttest\mass_case_description_train_set.csv");
            //console.writeline($"found {ddsmimages.count}");
            //ddsmimages.first().getnormalizedsizedcrop(1000).saveaspng("black.png");

            //            Serializer.Save("data.bin", DDSMImages);
            //            foreach (DdsmImage ddsmImage in DDSMImages)
            //            {
            //                Console.WriteLine($"{ddsmImage.PatientId} | {ddsmImage.ImageView} | {ddsmImage.BreastSide} | {ddsmImage.GetDcomCroppedImage().Width}x{ddsmImage.GetDcomCroppedImage().Height}");
            //            }
<<<<<<< HEAD
            List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"D:\Bryster\mass_case_description_test_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");


            imgInfo.ApplyContrastEnhancement(20);
            imgInfo.SaveAsPng("test.png");
=======
           /* List<DdsmImage> DDSMImages =
                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");

            var x = DDSMImages.First();
            x.GetDcomCroppedImage().PixelArray = x.GetDcomCroppedImage().PixelArray;
            x.GetNormalizedSizedCrop(1000).SaveAsPng("black.png");
            */
>>>>>>> dev

//            Serializer.Save("data.bin", DDSMImages);
//            foreach (DdsmImage ddsmImage in DDSMImages)
//            {
//                Console.WriteLine($"{ddsmImage.PatientId} | {ddsmImage.ImageView} | {ddsmImage.BreastSide} | {ddsmImage.GetDcomCroppedImage().Width}x{ddsmImage.GetDcomCroppedImage().Height}");
//            }

            // Lets render a picture:
            //            Console.WriteLine(DDSMImages[10].DcomMaskFilePath);

            //DDSMImages.First().GetDcomOriginalImage().SaveAsPng("original.png");
            DDSMImages.First().GetDcomMaskImage().SaveAsPng("mask.png");
            DDSMImages.First().GetDcomCroppedImage().SaveAsPng("cropped.png");

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