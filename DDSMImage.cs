using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

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

        public DDSMImage(int breastDensity, BreastSideEnum breastSide, ImageViewEnum imageView, int abnormalityId, MassShapesEnum massShape,
            MassMarginsEnum massMargins, Pathologies pathology, int subtlety, string dcomFilePath, string dcomMaskFilePath,
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

                MassShapesEnum massShape = GetMassShapeFromString(informations[5]);
                
                MassMarginsEnum massMargins = GetMassMarginsFromString(informations[6]);
                
                Pathologies pathology = Pathologies.BENIGN;
                if (informations[7].Equals("MALIGNANT")) pathology = Pathologies.MALIGNANT;
                if (informations[7].Equals("BENIGN_WITHOUT_CALLBACK")) pathology = Pathologies.BENIGN_WITHOUT_CALLBACK;

                if (!int.TryParse(informations[8], out var subtlety)) throw new Exception();
                String dcomFilePath = GetDcomFilePathFromString(informations[9]);
                (String, String) filePathsMaskAndCropped = GetDcomMaskAndCroppedPathsFromString(informations[10]);
                imagesToReturn.Add(new DDSMImage(breastDensity, breastSide, imageView, abnormalityId, massShape,
                    massMargins, pathology, subtlety, dcomFilePath, filePathsMaskAndCropped.Item1, filePathsMaskAndCropped.Item2));
            }

            return imagesToReturn;
        }

        private static MassMarginsEnum GetMassMarginsFromString(string orignalStr)
        {
            throw new NotImplementedException();
        }

        private static MassShapesEnum GetMassShapeFromString(string originalStr)
        {
            throw new NotImplementedException();
        }

        private static String GetDcomFilePathFromString(string s)
        {
            throw new NotImplementedException();
        }

        public static (String, String) GetDcomMaskAndCroppedPathsFromString(String originalPath)
        {
            throw new NotImplementedException();
            return (Mask: null, Cropped:null);
        }
    }
}