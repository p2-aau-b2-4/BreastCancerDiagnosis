using System;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Dicom;
using Dicom.IO.Buffer;
using Dicom.Imaging;

namespace DicomDisplayTest
{
  public static class DicomFileExtensions
  {
    public static byte[,] GetByteMatrix(this DicomFile f)
    {
      DicomPixelData pixelData = DicomPixelData.Create(f.Dataset);

      byte[] byteData = pixelData.GetFrame(0).Data;
      Console.WriteLine("bytes real: " + byteData.Length);

      int columns = f.Dataset.Get<int>(DicomTag.Columns);
      int rows = f.Dataset.Get<int>(DicomTag.Rows);

      Console.WriteLine("bytes calculated: " + (2 * rows) * columns);

      byte[,] byteMatrix = new byte[columns * 2, rows];

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns * 2; j++)
          byteMatrix[j, i] = byteData[(rows * i) + j];
      }
      return byteMatrix;
    }
    public static void SetByteMatrix(this DicomFile f, byte[,] byteMatrix)
    {
      int columns = byteMatrix.GetLength(0);
      int rows = byteMatrix.GetLength(1);
      
      Console.WriteLine(columns + " " + rows);

      byte[] byteArray = new byte[columns * rows];
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
	  Console.WriteLine(i + " " + j);
          byteArray[(i * rows) + j] = byteMatrix[j, i];
        }
      }
      MemoryByteBuffer byteBuffer = new MemoryByteBuffer(byteArray);

      f.Dataset.AddOrUpdatePixelData(DicomVR.OB, byteBuffer);
    }
  }
}
