using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DicomDisplayTest
{
    public class DDSMImage
    {
        enum BreatSideEnum
        {
            LEFT_BREAST,
            RIGHT_BREAST
        };

        enum ImageViewEnum
        {
            CC,
            MLO
        };

        enum MassShapesEnum
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

        enum MassMarginsEnum
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

        enum Pathologies
        {
            MALIGNANT,
            BENIGN,
            BENIGN_WITHOUT_CALLBACK
        };


        public int BreastDensity { get; set; }
        public int BreastSide { get; set; }
        public int ImageView { get; set; }
        public int AbnormalityId { get; set; }
        public int MassShape { get; set; }
        public int MassMargins { get; set; }
        public int Pathology { get; set; }
        public int Subtlety { get; set; }
        public String DcomFilePath { get; set; }
        public String DcomMaskFilePath { get; set; }
        public String DcomCroppedFilePath { get; set; }

        public DDSMImage(int breastDensity, int breastSide, int imageView, int abnormalityId, int massShape, int massMargins, int pathology, int subtlety, string dcomFilePath, string dcomMaskFilePath, string dcomCroppedFilePath)
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

        public List<DDSMImage> GetAllImagesFromCSVFile(String csvFilePath)
        {
            return null;
        }
    }
}