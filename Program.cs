using System;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Dicom;
using Dicom.Imaging;

namespace DicomDisplayTest
{
  class Program
  {
    static void Main(string[] args)
    {
      ImageManager.SetImplementation(WinFormsImageManager.Instance);
      var image = new DicomImage(@"e.dcm");
      image.RenderImage().AsClonedBitmap().Save(@"test1.bmp");
      int tis = 949494;
    }
  }
}
