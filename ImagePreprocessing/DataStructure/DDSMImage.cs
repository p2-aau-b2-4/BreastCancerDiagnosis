using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;
using Dicom;

namespace ImagePreprocessing
{
    class FileSizeComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return y.Length - x.Length > 0 ? 1 : y.Length - x.Length < 0 ? -1 : 0;
        }
    }

    [Serializable]
    public class DdsmImage
    {

        public enum ImageViewEnum
        {
            Mlo, 
            Cc
        }

        public enum Pathologies
        {
            Malignant,
            Benign,
            BenignWithoutCallback
        }

        public ImageViewEnum ImageView { get; }
        public Pathologies Pathology { get; }
        public String DcomFilePath { get; }
        public String DcomMaskFilePath { get; }
        public String DcomCroppedFilePath { get; }

        public virtual UShortArrayAsImage DcomOriginalImage
        {
            get { return DicomFile.Open(DcomFilePath).GetUshortImageInfo(); }
        }

        public virtual UByteArrayAsImage DcomMaskImage
        {
            get { return DicomFile.Open(DcomMaskFilePath).GetUByteImageInfo(); }
        }

        public UShortArrayAsImage DcomCroppedImage
        {
            get { return DicomFile.Open(DcomCroppedFilePath).GetUshortImageInfo(); }
        }

        private DdsmImage(ImageViewEnum imageView,
            Pathologies pathology, string dcomFilePath,
            string dcomMaskFilePath,
            string dcomCroppedFilePath)
        {
            ImageView = imageView;
            Pathology = pathology;
            DcomFilePath = dcomFilePath;
            DcomMaskFilePath = dcomMaskFilePath;
            DcomCroppedFilePath = dcomCroppedFilePath;
        }
        public DdsmImage(){}

        public static List<DdsmImage> GetAllImagesFromCsvFile(String csvFilePath)
        {
            List<DdsmImage> imagesToReturn = new List<DdsmImage>();
            
            TextReader reader = new StreamReader(csvFilePath);
            var csvReader = new CsvReader(reader);
            csvReader.Read(); //skip header
            while (csvReader.Read())
            {
                ImageViewEnum imageView = ImageViewEnum.Cc;
                if (csvReader.GetField(3).Equals("MLO")) imageView = ImageViewEnum.Mlo;
                
                Pathologies pathology = Pathologies.Benign;
                if (csvReader.GetField(9).Equals("MALIGNANT")) pathology = Pathologies.Malignant;
                if (csvReader.GetField(9).Equals("BENIGN_WITHOUT_CALLBACK")) pathology = Pathologies.BenignWithoutCallback;
                
                String dcomFilePath = GetDcomFilePathFromString(csvFilePath, csvReader.GetField(11));

                var (maskFilePath, croppedFilePath) =
                    GetDcomMaskAndCroppedPathsFromString(csvFilePath, csvReader.GetField(12));
                imagesToReturn.Add(new DdsmImage(imageView,pathology, dcomFilePath, maskFilePath, croppedFilePath));
            }
            return imagesToReturn;
        }

        private static string GetDcomFilePathFromString(string csvFilePath, string s)
        {
            csvFilePath = csvFilePath.Substring(0, csvFilePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

            StringBuilder stringBuilder = new StringBuilder(csvFilePath);
            String[] folders = s.Split("/");
            stringBuilder.Append(folders[0]);
            String folderFound = Directory.GetDirectories(stringBuilder.ToString())[0];
            folderFound = Directory.GetDirectories(folderFound)[0];
            //todo maybe check if filename is equal to last element of folders string array.
            return Directory.GetFiles(folderFound)[0];
        }

        public static ( string, string) GetDcomMaskAndCroppedPathsFromString(string csvFilePath, string originalPath)
        {
            // so the logic is:
            // 1. First explode the originalPathString, and navigate into first folder.
            // 2. If there is 2 folders there, then one contains "mask", the other "cropped" as a part of the name in a subfolder.
            // 2a. Then in each of those folder is the corresponding dcom file
            // 3. else, there is only one folder. Go down as far as you can go, and then find two files at the end.
            // Look at originalPath filename, that should be the filepath of the cropped image, the other image is the mask.
            // lets implement
            int lastIndexOf = csvFilePath.LastIndexOf(@"\", StringComparison.Ordinal) >
                              csvFilePath.LastIndexOf(@"/", StringComparison.Ordinal)
                ? csvFilePath.LastIndexOf(@"\", StringComparison.Ordinal)
                : csvFilePath.LastIndexOf(@"/", StringComparison.Ordinal);
            // slash is used in linux, backslash is often used in windows. So lets find the last occurence of either.
            // They are never equal, since 2 characters cant be at the same place.
            csvFilePath = csvFilePath.Substring(0, lastIndexOf + 1);

            String[] folders = originalPath.Split("/");
            String[] foldersInFirstFolder = Directory.GetDirectories(csvFilePath + folders[0].Replace("\"",""));

            String maskFilePath = null;
            String croppedFilePath = null;

            if (foldersInFirstFolder.Length > 1)
            {
                // step 2
                // lets first find the mask image:
                foreach (String folderInFirstFolder in foldersInFirstFolder)
                {
                    String subfolder = Directory.GetDirectories(folderInFirstFolder)[0];
                    if (subfolder.Contains("mask"))
                    {
                        maskFilePath = Directory.GetFiles(subfolder)[0];
                    }
                    else if (subfolder.Contains("cropped"))
                    {
                        croppedFilePath = Directory.GetFiles(subfolder)[0];
                    }
                    else
                    {
                        throw new DirectoryNotFoundException();
                    }
                }
            }
            else
            {
                // step 3;
                String activeFolder = foldersInFirstFolder[0];
                while (Directory.GetDirectories(activeFolder).Length > 0)
                    activeFolder = Directory.GetDirectories(activeFolder)[0];
                String[] files = Directory.GetFiles(activeFolder);

                List<FileInfo> dcmFilesFound = new List<FileInfo>();
                foreach (String file in files)
                {
                    if (file.EndsWith(".dcm"))
                    {
                        dcmFilesFound.Add(new FileInfo(file));
                    }
                }

                dcmFilesFound.Sort(new FileSizeComparer());
                maskFilePath = dcmFilesFound[0].FullName;
                croppedFilePath = dcmFilesFound[1].FullName;
            }

            return (maskFilePath, croppedFilePath);
        }
    }
}