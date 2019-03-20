using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Dicom;
using DicomDisplayTest;

namespace DICOMTests
{
    public class DdsmImage
    {
        public enum BreastSideEnum
        {
            LeftBreast,
            RightBreast
        }

        public enum ImageViewEnum
        {
            Mlo,
            Cc
        }

        public enum MassShapesEnum
        {
            ArchitecturalDistortion,
            AsymmetricBreastTissue,
            FocalAsymmetricDensity,
            IrregularArchitecturalDistortion,
            IrregularFocalAsymmetricDensity,
            Irregular,
            Lobulated,
            LobulatedArchitecturalDistortion,
            LobulatedIrregular,
            LobulatedLymphNode,
            LobulatedOval,
            LymphNode,
            NotAvailable,
            OvalLymphNode,
            Oval,
            Round,
            RoundIrregularArchitecturalDistortion,
            RoundLobulated,
            RoundOval
        }

        public enum MassMarginsEnum
        {
            Circumscribed,
            CircumscribedIllDefined,
            CircumscribedMicrolobulated,
            CircumscribedObscured,
            IllDefined,
            IllDefinedSpiculated,
            MicrolobulatedIllDefined,
            Microlobulated,
            MicrolobulatedIllDefinedSpiculated,
            MicrolobulatedSpiculated,
            NotAvailable,
            ObscuredIllDefined,
            Obscured,
            ObscuredIllDefinedSpiculated,
            ObscuredSpiculated,
            Spiculated
        }

        public enum Pathologies
        {
            Malignant,
            Benign,
            BenignWithoutCallback
        }

        public String PatientId { get; }
        public int BreastDensity { get; }
        public BreastSideEnum BreastSide { get; }
        public ImageViewEnum ImageView { get; }
        public int AbnormalityId { get; }
        public MassShapesEnum MassShape { get; }
        public MassMarginsEnum MassMargins { get; }
        public int Assessment { get; }
        public Pathologies Pathology { get; }
        public int Subtlety { get; }
        public String DcomFilePath { get; }
        public String DcomMaskFilePath { get; }
        public String DcomCroppedFilePath { get; }

        public DdsmImage(String patientId, int breastDensity, BreastSideEnum breastSide, ImageViewEnum imageView,
            int abnormalityId,
            MassShapesEnum massShape,
            MassMarginsEnum massMargins, int assessment, Pathologies pathology, int subtlety, string dcomFilePath,
            string dcomMaskFilePath,
            string dcomCroppedFilePath)
        {
            PatientId = patientId;
            BreastDensity = breastDensity;
            BreastSide = breastSide;
            ImageView = imageView;
            AbnormalityId = abnormalityId;
            MassShape = massShape;
            MassMargins = massMargins;
            Assessment = assessment;
            Pathology = pathology;
            Subtlety = subtlety;
            DcomFilePath = dcomFilePath;
            DcomMaskFilePath = dcomMaskFilePath;
            DcomCroppedFilePath = dcomCroppedFilePath;
        }

        public static List<DdsmImage> GetAllImagesFromCsvFile(String csvFilePath)
        {
            List<DdsmImage> imagesToReturn = new List<DdsmImage>();
            List<String> lines = File.ReadAllLines(csvFilePath).ToList();
            lines.RemoveAt(0);
            foreach (String line in lines)
            {
                String[] informations = line.Split(',');
                if (informations.Length == 1) continue; // some lines are encoded with no info, lets skip those.
                String patientId = informations[0];

                if (!int.TryParse(informations[1], out var breastDensity)) throw new Exception();

                BreastSideEnum breastSide = BreastSideEnum.RightBreast;
                if (informations[2].Equals("LEFT")) breastSide = BreastSideEnum.LeftBreast;

                ImageViewEnum imageView = ImageViewEnum.Cc;
                if (informations[3].Equals("MLO")) imageView = ImageViewEnum.Mlo;

                if (!int.TryParse(informations[4], out var abnormalityId)) throw new Exception();

                //No reason to read field nr 5, as it just contains abnormality type, which is always "mass"

                MassShapesEnum massShape = GetMassShapeFromString(informations[6]);

                MassMarginsEnum massMargins = GetMassMarginsFromString(informations[7]);

                if (!int.TryParse(informations[8], out var abnormality)) throw new Exception();

                Pathologies pathology = Pathologies.Benign;
                if (informations[9].Equals("MALIGNANT")) pathology = Pathologies.Malignant;
                if (informations[9].Equals("BENIGN_WITHOUT_CALLBACK")) pathology = Pathologies.BenignWithoutCallback;

                if (!int.TryParse(informations[10], out var subtlety)) throw new Exception();
                String dcomFilePath = GetDcomFilePathFromString(csvFilePath, informations[11]);

                (String, String) filePathsMaskAndCropped =
                    GetDcomMaskAndCroppedPathsFromString(csvFilePath, informations[12]);
                imagesToReturn.Add(new DdsmImage(patientId, breastDensity, breastSide, imageView, abnormalityId,
                    massShape,
                    massMargins, abnormality, pathology, subtlety, dcomFilePath, filePathsMaskAndCropped.Item1,
                    filePathsMaskAndCropped.Item2));
            }

            return imagesToReturn;
        }

        private static MassMarginsEnum GetMassMarginsFromString(string originalStr)
        {
            switch (originalStr)
            {
                case "CIRCUMSCRIBED": return MassMarginsEnum.Circumscribed;
                case "CIRCUMSCRIBED-ILL_DEFINED": return MassMarginsEnum.CircumscribedIllDefined;
                case "CIRCUMSCRIBED-MICROLOBULATED": return MassMarginsEnum.CircumscribedMicrolobulated;
                case "CIRCUMSCRIBED-OBSCURED": return MassMarginsEnum.CircumscribedObscured;
                case "ILL_DEFINED": return MassMarginsEnum.IllDefined;
                case "ILL_DEFINED-SPICULATED": return MassMarginsEnum.IllDefinedSpiculated;
                case "MICROLOBULATED": return MassMarginsEnum.Microlobulated;
                case "MICROLOBULATED-ILL_DEFINED": return MassMarginsEnum.MicrolobulatedIllDefined;
                case "MICROLOBULATED-ILL_DEFINED-SPICULATED":
                    return MassMarginsEnum.MicrolobulatedIllDefinedSpiculated;
                case "MICROLOBULATED-SPICULATED": return MassMarginsEnum.MicrolobulatedSpiculated;
                case "N/A": return MassMarginsEnum.NotAvailable;
                case "OBSCURED": return MassMarginsEnum.Obscured;
                case "OBSCURED-ILL_DEFINED-SPICULATED": return MassMarginsEnum.ObscuredIllDefinedSpiculated;
                case "OBSCURED-ILL_DEFINED": return MassMarginsEnum.ObscuredIllDefined;
                case "OBSCURED-SPICULATED": return MassMarginsEnum.ObscuredSpiculated;
                case "SPICULATED": return MassMarginsEnum.Spiculated;
                default:
                    throw new ArgumentException($"Input {originalStr} was not found in table", nameof(originalStr));
            }
        }

        private static MassShapesEnum GetMassShapeFromString(string originalStr)
        {
            switch (originalStr)
            {
                case "ARCHITECTURAL_DISTORTION": return MassShapesEnum.ArchitecturalDistortion;
                case "ASYMMETRIC_BREAST_TISSUE": return MassShapesEnum.AsymmetricBreastTissue;
                case "FOCAL_ASYMMETRIC_DENSITY": return MassShapesEnum.FocalAsymmetricDensity;
                case "IRREGULAR": return MassShapesEnum.Irregular;
                case "IRREGULAR-ARCHITECTURAL_DISTORTION": return MassShapesEnum.IrregularArchitecturalDistortion;
                case "IRREGULAR-FOCAL_ASYMMETRIC_DENSITY": return MassShapesEnum.IrregularFocalAsymmetricDensity;
                case "LOBULATED": return MassShapesEnum.Lobulated;
                case "LOBULATED-ARCHITECTURAL_DISTORTION": return MassShapesEnum.LobulatedArchitecturalDistortion;
                case "LOBULATED-IRREGULAR": return MassShapesEnum.LobulatedIrregular;
                case "LOBULATED-LYMPH_NODE": return MassShapesEnum.LobulatedLymphNode;
                case "LOBULATED-OVAL": return MassShapesEnum.LobulatedOval;
                case "LYMPH_NODE": return MassShapesEnum.LymphNode;
                case "N/A": return MassShapesEnum.NotAvailable;
                case "OVAL": return MassShapesEnum.Oval;
                case "OVAL-LYMPH_NODE": return MassShapesEnum.OvalLymphNode;
                case "ROUND": return MassShapesEnum.Round;
                case "ROUND-IRREGULAR-ARCHITECTURAL_DISTORTION":
                    return MassShapesEnum.RoundIrregularArchitecturalDistortion;
                case "ROUND-LOBULATED": return MassShapesEnum.RoundLobulated;
                case "ROUND-OVAL": return MassShapesEnum.RoundOval;
                default:
                    throw new ArgumentException($"Input {originalStr} was not found in table", nameof(originalStr));
            }
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
            // 3. else, there is only one folder. Go down as far as you can go, and then find two files at the end. Look at originalPath filename, that should be the filepath of the cropped image, the other image is the mask.
            // lets implement
            int lastIndexOf = csvFilePath.LastIndexOf(@"\", StringComparison.Ordinal) >
                              csvFilePath.LastIndexOf(@"/", StringComparison.Ordinal)
                ? csvFilePath.LastIndexOf(@"\", StringComparison.Ordinal)
                : csvFilePath.LastIndexOf(@"/", StringComparison.Ordinal);
            // slash is used in linux, backslash is often used in windows. So lets find the last occurence of either.
            // They are never equal, since 2 characters cant be at the same place.
            csvFilePath = csvFilePath.Substring(0, lastIndexOf + 1);

            String[] folders = originalPath.Split("/");
            String[] foldersInFirstFolder = Directory.GetDirectories(csvFilePath + folders[0]);

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

            return (maskFilePath, croppedFilePath);
        }


        public UshortArrayAsImage GetDcomOriginalImage()
        {
            return DicomFile.Open(DcomFilePath).GetUshortImageInfo();
        }

        public UshortArrayAsImage GetDcomMaskImage()
        {
            return DicomFile.Open(DcomMaskFilePath).GetUshortImageInfo();
        }

        public UshortArrayAsImage GetDcomCroppedImage()
        {
            return DicomFile.Open(DcomCroppedFilePath).GetUshortImageInfo();
        }
    }
}