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
        public static UshortImageInfo GetUshortImageInfo(this DicomFile f)
        {
            return new UshortImageInfo(GetUshortMatrix(f));
        }
        
        public static ushort[,] GetUshortMatrix(this DicomFile f)
        {
            DicomPixelData pixelData = DicomPixelData.Create(f.Dataset);
            int columns = f.Dataset.GetValue<int>(DicomTag.Columns, 0);
            int rows = f.Dataset.GetValue<int>(DicomTag.Rows, 0);

            byte[] byteData = pixelData.GetFrame(0).Data;
            Console.WriteLine($"Expected length {columns}x{rows}={columns * rows}");
            Console.WriteLine(byteData.Length);

            ushort[,] ushortMatrix = new ushort[columns, rows];

            for (var i = 0; i < byteData.Length; i = i + 2)
            {
                ushortMatrix[i / 2 % columns,i / 2 / columns] = BitConverter.ToUInt16(byteData, i);
            }

            return ushortMatrix;
        }

        public static byte[,] GetByteMatrix(this DicomFile f)
        {
            DicomPixelData pixelData = DicomPixelData.Create(f.Dataset);

            byte[] byteData = pixelData.GetFrame(0).Data;
            Console.WriteLine("bytes real: " + byteData.Length);

            int columns = f.Dataset.GetValue<int>(DicomTag.Columns, 0);
            int rows = f.Dataset.GetValue<int>(DicomTag.Rows, 0);

            Console.WriteLine("bytes calculated: " + (2 * rows) * columns);

            byte[,] byteMatrix = new byte[columns * 2, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns * 2; j++)
                    byteMatrix[j, i] = byteData[(rows * i) + j];
            }

            return byteMatrix;
        }

        public static void SetByteMatrix(this DicomFile f, byte[,] byteMatrix)
        {
            int columns = byteMatrix.GetLength(0);
            int rows = byteMatrix.GetLength(1);

            byte[] byteArray = new byte[columns * rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.WriteLine(i + " " + j);
                    byteArray[(i * rows) + j] = byteMatrix[j, i];
                }
            }

            MemoryByteBuffer byteBuffer = new MemoryByteBuffer(byteArray);

            f.Dataset.AddOrUpdatePixelData(DicomVR.OB, byteBuffer);
        }
    }
}