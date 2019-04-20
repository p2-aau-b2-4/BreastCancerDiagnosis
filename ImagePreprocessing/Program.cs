using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Dicom;
using Dicom.Imaging;
using Microsoft.Win32;

namespace ImagePreprocessing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ConfigurationManager.AppSettings["myKey"]);
            var img = DicomFile.Open(@"e.dcm");
            UshortArrayAsImage imgInfo = img.GetUshortImageInfo();
            imgInfo.SaveAsPng("eINFO.PNG");

            
            imgInfo.PixelArray = Contrast.Equalization(imgInfo.PixelArray, 25);
            imgInfo.SaveAsPng("eINFOContrast.png");    //REMOVE
            var imgOverlay = imgInfo.Edge(1000);
            imgOverlay.SaveAsPng("OliverErEnNar.png");
            
            //list<ddsmimage> ddsmimages =
            //    ddsmimage.getallimagesfromcsvfile(@"e:\brysttest\mass_case_description_train_set.csv");
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


            /*
            var img = DicomFile.Open(@"e.dcm");
            UshortImageInfo imgInfo = img.GetUshortImageInfo();
            //imgInfo.Render("brystDef.png");

            //imgInfo.ApplyNoiseFilter(1);
            //imgInfo.Render("brystNoiseFilter.png");

            
            ushort[,] pixels = new ushort[imgInfo.PixelArray.GetLength(0),imgInfo.PixelArray.GetLength(1)];
            pixels = imgInfo.PixelArray;

            var img2 = new UshortImageInfo(Contrast.Equalization(pixels,0.0));
            img2.Render("brystContrast0.png");
            
            var img3 = new UshortImageInfo(Contrast.Equalization(pixels,10.0));
            img3.Render("brystContrast10.png");
            
            var img4 = new UshortImageInfo(Contrast.Equalization(pixels,20.0));
            img4.Render("brystContrast20.png");
            
            var img5 = new UshortImageInfo(Contrast.Equalization(pixels,30.0));
            img5.Render("brystContrast30.png");
            
            var img6 = new UshortImageInfo(Contrast.Equalization(pixels,40.0));
            img6.Render("brystContrast40.png");
            
            var img7 = new UshortImageInfo(Contrast.Equalization(pixels,50.0));
            img7.Render("brystContrast50.png");
            
            var img8 = new UshortImageInfo(Contrast.Equalization(pixels,60.0));
            img8.Render("brystContrast60.png");
            
            var img9 = new UshortImageInfo(Contrast.Equalization(pixels,70.0));
            img9.Render("brystContrast70.png");
            
            var img10 = new UshortImageInfo(Contrast.Equalization(pixels,80.0));
            img10.Render("brystContrast80.png");
            
            var img11 = new UshortImageInfo(Contrast.Equalization(pixels,90.0));
            img11.Render("brystContrast90.png");
            
            var img12 = new UshortImageInfo(Contrast.Equalization(pixels,100.0));
            img12.Render("brystContrast100.png");
            
            var img13 = new UshortImageInfo(Contrast.Equalization(pixels,-10.0));
            img13.Render("brystContrast-10.png");
            
            var img14 = new UshortImageInfo(Contrast.Equalization(pixels,-40.0));
            img14.Render("brystContrast-40.png");
            
            var img15 = new UshortImageInfo(Contrast.Equalization(pixels,-80.0));
            img15.Render("brystContrast-80.png");
            
            /*Bitmap overlayToAdd = new Bitmap(550,550);
            for (int x = 200; x < 500; x++)
            {
                overlayToAdd.SetPixel(x,200,Color.Red);
                overlayToAdd.SetPixel(x,400,Color.Red);
            }
            for (int y = 200; y < 400; y++)
            {
                overlayToAdd.SetPixel(200,y,Color.Red);
                overlayToAdd.SetPixel(500,y,Color.Red);
            }
            imgInfo.AddOverlay(overlayToAdd);
            
            imgInfo.Render("bryst.png");*/
        }
    }
}