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
      var img = DicomFile.Open(@"e.dcm");
      var bytes = img.GetByteMatrix();
      img.SetByteMatrix(bytes);

      img.Save(@"e2.dcm");

      ImageManager.SetImplementation(WinFormsImageManager.Instance);
      var image = new DicomImage(@"e.dcm");
      image.RenderImage().AsClonedBitmap().Save(@"test1.bmp");
      var bitmap = new Bitmap(image.Width, image.Height);
      bitmap = Contrast.equalization(image);
      bitmap.Save("test2.bmp");
    }
  }
}
