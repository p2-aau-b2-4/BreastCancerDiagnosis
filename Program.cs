using System;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Dicom;
using Dicom.Imaging;
using Microsoft.Win32;

namespace DicomDisplayTest
{
    class Program
    {
        static void Main(string[] args)
        {
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
            
            imgInfo.Render("bryst.png");
        }
    }
}