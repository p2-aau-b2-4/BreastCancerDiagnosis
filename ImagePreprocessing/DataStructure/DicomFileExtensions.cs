using Dicom;
using Dicom.Imaging;

namespace ImagePreprocessing
{
    public static class DicomFileExtensions
    {
        /// <summary>
        /// Extracts DICOM file data and saves it as a UShortArrayAsImage.
        /// </summary>
        /// <param name="f">A DICOM file</param>
        /// <returns>A UShortArrayAsImage</returns>
        public static UShortArrayAsImage GetUshortImageInfo(this DicomFile f)
        {
            DicomPixelData pixelData = DicomPixelData.Create(f.Dataset);
            int columns = f.Dataset.GetValue<int>(DicomTag.Columns, 0);
            int rows = f.Dataset.GetValue<int>(DicomTag.Rows, 0);

            byte[] byteData = pixelData.GetFrame(0).Data;
            return new UShortArrayAsImage(byteData, columns, rows);
        }
        
        /// <summary>
        /// Extracts DICOM file data and saves it as a UByteArrayAsImage.
        /// </summary>
        /// <param name="f">A DICOM file</param>
        /// <returns>A UByteArrayAsImage</returns>
        public static UByteArrayAsImage GetUByteImageInfo(this DicomFile f)
        {
            DicomPixelData pixelData = DicomPixelData.Create(f.Dataset);
            int columns = f.Dataset.GetValue<int>(DicomTag.Columns, 0);
            int rows = f.Dataset.GetValue<int>(DicomTag.Rows, 0);

            byte[] byteData = pixelData.GetFrame(0).Data;
            return new UByteArrayAsImage(byteData, columns, rows);
        } 
    }
}