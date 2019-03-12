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
            img.GetUshortImageInfo().Render("bryst.png");
        }
    }
}