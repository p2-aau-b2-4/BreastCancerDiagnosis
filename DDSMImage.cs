using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CharLS;
using CSJ2K.j2k.quantization;
using Dicom;

namespace DicomDisplayTest
{
    public class DDSMImage
    {
        public enum BreastSideEnum
        {
            LEFT_BREAST,
            RIGHT_BREAST
        };

        public enum ImageViewEnum
        {
            CC,
            MLO
        };

        public enum MassShapesEnum
        {
            ARCHITECTURAL_DISTORTION,
            ASYMMETRIC_BREAST_TISSUE,
            FOCAL_ASYMMETRIC_DENSITY,
            IRREGULAR,
            IRREGULAR_ARCHITECTURAL_DISTORTION,
            IRREGULAR_FOCAL_ASYMMETRIC_DENSITY,
            LOBULATED,
            LOBULATED_ARCHITECTURAL_DISTORTION,
            LOBULATED_IRREGULAR,
            LOBULATED_LYMPH_NODE,
            LOBULATED_OVAL,
            LYMPH_NODE,
            NOT_AVAILABLE,
            OVAL,
            OVAL_LYMPH_NODE,
            ROUND,
            ROUND_IRREGULAR_ARCHITECTURAL_DISTORTION,
            ROUND_LOBULATED,
            ROUND_OVAL
        };

        public enum MassMarginsEnum
        {
            CIRCUMSCRIBED,
            CIRCUMSCRIBED_ILL_DEFINED,
            CIRCUMSCRIBED_MICROLOBULATED,
            CIRCUMSCRIBED_OBSCURED,
            ILL_DEFINED,
            ILL_DEFINED_SPICULATED,
            MICROLOBULATED,
            MICROLOBULATED_ILL_DEFINED,
            MICROLOBULATED_ILL_DEFINED_SPICULATED,
            MICROLOBULATED_SPICULATED,
            NOT_AVAILABLE,
            OBSCURED,
            OBSCURED_ILL_DEFINED,
            OBSCURED_ILL_DEFINED_SPICULATED,
            OBSCURED_SPICULATED,
            SPICULATED
        };

        public enum Pathologies
        {
            MALIGNANT,
            BENIGN,
            BENIGN_WITHOUT_CALLBACK
        };


        public int BreastDensity { get; set; }
        public BreastSideEnum BreastSide { get; set; }
        public ImageViewEnum ImageView { get; set; }
        public int AbnormalityId { get; set; }
        public MassShapesEnum MassShape { get; set; }
        public MassMarginsEnum MassMargins { get; set; }
        public Pathologies Pathology { get; set; }
        public int Subtlety { get; set; }
        public String DcomFilePath { get; set; }
        public String DcomMaskFilePath { get; set; }
        public String DcomCroppedFilePath { get; set; }

        public DDSMImage(int breastDensity, BreastSideEnum breastSide, ImageViewEnum imageView, int abnormalityId,
            MassShapesEnum massShape,
            MassMarginsEnum massMargins, Pathologies pathology, int subtlety, string dcomFilePath,
            string dcomMaskFilePath,
            string dcomCroppedFilePath)
        {
            BreastDensity = breastDensity;
            BreastSide = breastSide;
            ImageView = imageView;
            AbnormalityId = abnormalityId;
            MassShape = massShape;
            MassMargins = massMargins;
            Pathology = pathology;
            Subtlety = subtlety;
            DcomFilePath = dcomFilePath;
            DcomMaskFilePath = dcomMaskFilePath;
            DcomCroppedFilePath = dcomCroppedFilePath;
        }

        public static List<DDSMImage> GetAllImagesFromCSVFile(String csvFilePath)
        {
            List<DDSMImage> imagesToReturn = new List<DDSMImage>();
            List<String> lines = System.IO.File.ReadAllLines(csvFilePath).ToList();
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                String[] informations = line.Split(',');
                if (informations.Length == 1) continue;

                if (!int.TryParse(informations[1], out var breastDensity)) throw new Exception();

                BreastSideEnum breastSide = BreastSideEnum.RIGHT_BREAST;
                if (informations[2].Equals("LEFT")) breastSide = BreastSideEnum.LEFT_BREAST;

                ImageViewEnum imageView = ImageViewEnum.CC;
                if (informations[3].Equals("MLO")) imageView = ImageViewEnum.MLO;

                if (!int.TryParse(informations[4], out var abnormalityId)) throw new Exception();

                //abnormality type?

                MassShapesEnum massShape = GetMassShapeFromString(informations[6]);

                MassMarginsEnum massMargins = GetMassMarginsFromString(informations[7]);
                
                // assesment?

                Pathologies pathology = Pathologies.BENIGN;
                if (informations[9].Equals("MALIGNANT")) pathology = Pathologies.MALIGNANT;
                if (informations[9].Equals("BENIGN_WITHOUT_CALLBACK")) pathology = Pathologies.BENIGN_WITHOUT_CALLBACK;

                if (!int.TryParse(informations[10], out var subtlety)) throw new Exception();
                String dcomFilePath = GetDcomFilePathFromString(csvFilePath, informations[11]);
                
                (String, String) filePathsMaskAndCropped = GetDcomMaskAndCroppedPathsFromString(csvFilePath, informations[12]);
                imagesToReturn.Add(new DDSMImage(breastDensity, breastSide, imageView, abnormalityId, massShape,
                    massMargins, pathology, subtlety, dcomFilePath, filePathsMaskAndCropped.Item1,
                    filePathsMaskAndCropped.Item2));
            }

            return imagesToReturn;
        }

        private static MassMarginsEnum GetMassMarginsFromString(string originalStr)
        {
            switch (originalStr)
            {
                case "CIRCUMSCRIBED": return MassMarginsEnum.CIRCUMSCRIBED;
                case "CIRCUMSCRIBED-ILL_DEFINED": return MassMarginsEnum.CIRCUMSCRIBED_ILL_DEFINED;
                case "CIRCUMSCRIBED-MICROLOBULATED": return MassMarginsEnum.CIRCUMSCRIBED_MICROLOBULATED;
                case "CIRCUMSCRIBED-OBSCURED": return MassMarginsEnum.CIRCUMSCRIBED_OBSCURED;
                case "ILL_DEFINED": return MassMarginsEnum.ILL_DEFINED;
                case "ILL_DEFINED-SPICULATED": return MassMarginsEnum.ILL_DEFINED_SPICULATED;
                case "MICROLOBULATED": return MassMarginsEnum.MICROLOBULATED;
                case "MICROLOBULATED-ILL_DEFINED": return MassMarginsEnum.MICROLOBULATED_ILL_DEFINED;
                case "MICROLOBULATED-ILL_DEFINED-SPICULATED":
                    return MassMarginsEnum.MICROLOBULATED_ILL_DEFINED_SPICULATED;
                case "MICROLOBULATED-SPICULATED": return MassMarginsEnum.MICROLOBULATED_SPICULATED;
                case "N/A": return MassMarginsEnum.NOT_AVAILABLE;
                case "OBSCURED": return MassMarginsEnum.OBSCURED;
                case "OBSCURED-ILL_DEFINED-SPICULATED": return MassMarginsEnum.OBSCURED_ILL_DEFINED_SPICULATED;
                case "OBSCURED-ILL_DEFINED": return MassMarginsEnum.OBSCURED_ILL_DEFINED;
                case "OBSCURED-SPICULATED": return MassMarginsEnum.OBSCURED_ILL_DEFINED;
                case "SPICULATED": return MassMarginsEnum.SPICULATED;
                default:
                    throw new ArgumentException($"Input {originalStr} was not found in table", nameof(originalStr));
            }
        }

        private static MassShapesEnum GetMassShapeFromString(string originalStr)
        {
            switch (originalStr)
            {
                case "ARCHITECTURAL_DISTORTION": return MassShapesEnum.ARCHITECTURAL_DISTORTION;
                case "ASYMMETRIC_BREAST_TISSUE": return MassShapesEnum.ASYMMETRIC_BREAST_TISSUE;
                case "FOCAL_ASYMMETRIC_DENSITY": return MassShapesEnum.FOCAL_ASYMMETRIC_DENSITY;
                case "IRREGULAR": return MassShapesEnum.IRREGULAR;
                case "IRREGULAR-ARCHITECTURAL_DISTORTION": return MassShapesEnum.IRREGULAR_ARCHITECTURAL_DISTORTION;
                case "IRREGULAR-FOCAL_ASYMMETRIC_DENSITY": return MassShapesEnum.IRREGULAR_FOCAL_ASYMMETRIC_DENSITY;
                case "LOBULATED": return MassShapesEnum.LOBULATED;
                case "LOBULATED-ARCHITECTURAL_DISTORTION": return MassShapesEnum.LOBULATED_ARCHITECTURAL_DISTORTION;
                case "LOBULATED-IRREGULAR": return MassShapesEnum.LOBULATED_IRREGULAR;
                case "LOBULATED-LYMPH_NODE": return MassShapesEnum.LOBULATED_LYMPH_NODE;
                case "LOBULATED-OVAL": return MassShapesEnum.LOBULATED_OVAL;
                case "LYMPH_NODE": return MassShapesEnum.LYMPH_NODE;
                case "N/A": return MassShapesEnum.NOT_AVAILABLE;
                case "OVAL": return MassShapesEnum.OVAL;
                case "OVAL-LYMPH_NODE": return MassShapesEnum.OVAL_LYMPH_NODE;
                case "ROUND": return MassShapesEnum.ROUND;
                case "ROUND-IRREGULAR-ARCHITECTURAL_DISTORTION":
                    return MassShapesEnum.ROUND_IRREGULAR_ARCHITECTURAL_DISTORTION;
                case "ROUND-LOBULATED": return MassShapesEnum.ROUND_LOBULATED;
                case "ROUND-OVAL": return MassShapesEnum.ROUND_OVAL;
                default:
                    throw new ArgumentException($"Input {originalStr} was not found in table", nameof(originalStr));
            }
        }

        private static string GetDcomFilePathFromString(string csvFilePath, string s)
        {
            csvFilePath = csvFilePath.Substring(0, csvFilePath.LastIndexOf(@"\")+1);
            
            StringBuilder stringBuilder = new StringBuilder(csvFilePath);
            String[] folders = s.Split("/");
            stringBuilder.Append(folders[0]);
            String folderFound = System.IO.Directory.GetDirectories(stringBuilder.ToString())[0];
            folderFound = System.IO.Directory.GetDirectories(folderFound)[0];
            //todo maybe check if filename is equal to last element of folders string array.
            return System.IO.Directory.GetFiles(folderFound)[0];
        }

        public static ( string, string) GetDcomMaskAndCroppedPathsFromString(string csvFilePath, string originalPath)
        {
            // so the logic is:
            // 1. First explode the originalPathString, and navigate into first folder.
            // 2. If there is 2 folders there, then one contains "mask", the other "cropped" as a part of the name in a subfolder.
            // 2a. Then in each of those folder is the corresponding dcom file
            // 3. else, there is only one folder. Go down as far as you can go, and then find two files at the end. Look at originalPath filename, that should be the filepath of the cropped image, the other image is the mask.
            // lets implement
            csvFilePath = csvFilePath.Substring(0, csvFilePath.LastIndexOf(@"\")+1);
            
            String[] folders = originalPath.Split("/");
            String[] foldersInFirstFolder = System.IO.Directory.GetDirectories(csvFilePath+folders[0]);

            String maskFilePath = null;
            String croppedFilePath = null;

            if (foldersInFirstFolder.Length > 1)
            {
                // step 2
                // lets first find the mask image:
                foreach(String folderInFirstFolder in foldersInFirstFolder)
                {
                    String subfolder = System.IO.Directory.GetDirectories(folderInFirstFolder)[0];
                    if(subfolder.Contains("mask"))
                    {
                        maskFilePath = System.IO.Directory.GetFiles(subfolder)[0];
                    }
                    else if(subfolder.Contains("cropped"))
                    {
                        croppedFilePath = System.IO.Directory.GetFiles(subfolder)[0];
                    }
                    else
                    {
                        throw new System.IO.DirectoryNotFoundException();
                    }
                }
            }
            else
            {
                // step 3;
                String activeFolder = foldersInFirstFolder[0];
                while (System.IO.Directory.GetDirectories(activeFolder).Length > 0)
                    activeFolder = System.IO.Directory.GetDirectories(activeFolder)[0];
                String[] files = System.IO.Directory.GetFiles(activeFolder);
                foreach (String file in files)
                {
                    if (file.EndsWith(folders[folders.Length - 1]))
                    {
                        maskFilePath = file;
                    }
                    else if (file.EndsWith(".dcm"))
                    {
                        croppedFilePath = file;
                    }
                }

                //throw new NotImplementedException();
            }
            return (Mask: maskFilePath, Cropped: croppedFilePath);
        }


        public UshortImageInfo GetDcomOriginalImage()
        {
            return DicomFile.Open(DcomFilePath).GetUshortImageInfo();
        }
        public UshortImageInfo GetDcomMaskImage()
        {
            return DicomFile.Open(DcomMaskFilePath).GetUshortImageInfo();
        }
        public UshortImageInfo GetDcomCroppedImage()
        {
            return DicomFile.Open(DcomCroppedFilePath).GetUshortImageInfo();
        }
    }
}