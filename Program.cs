using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Dicom;
using Dicom.Imaging;
using DICOMTests;
using Microsoft.Win32;

namespace DicomDisplayTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DdsmImage> DDSMImages = 
                DdsmImage.GetAllImagesFromCsvFile(@"E:\BrystTest\mass_case_description_train_set.csv");
            Console.WriteLine($"Found {DDSMImages.Count}");
            
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
            imgInfo.Render("brystDef.png");

            imgInfo.ApplyNoiseFilter(1);
            imgInfo.Render("brystNoiseFilter.png");
            
            Bitmap overlayToAdd = new Bitmap(550,550);
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