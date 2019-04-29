using System;
using System.Collections.Generic;
using System.Configuration;
using ImagePreprocessing;

namespace Training
{
    class Program
    {
        static void Main(string[] args)
        {
            // lets first get all the training data
            List<DdsmImage> ddsmImages =
                DdsmImage.GetAllImagesFromCsvFile(ConfigurationManager.AppSettings["trainingSetCsvPath"]);
            Console.WriteLine($"Found {ddsmImages.Count} sets..");

            // lets apply the relevant image preprocessing to every image
            foreach (var ddsmImage in ddsmImages)
            {
                var img = ddsmImage.DcomOriginalImage;
                //if (Convert.ToBoolean(ConfigurationManager.AppSettings["doContrast"]))
                //    img.ApplyContrastEnhancement(
                //        Convert.ToInt32(ConfigurationManager.AppSettings["contrastThreshold"]));

                //if (Convert.ToBoolean(ConfigurationManager.AppSettings["doRemoveBreastMuscle"]))
                 //   img.RemoveBreastMuscle();
                
                // now lets resize it, according to the dataset mask
                //img.CropFromMask(ddsmImage.GetDcomMaskImage(),ConfigurationManager.AppSettings["safeDistanceCropFromMask"]);

                //if (Convert.ToBoolean(ConfigurationManager.AppSettings["doNormalizing"]))
                    //img.Normalize();
                
                // todo save this processed image somehow???
            }
            
            // lets give pca this data:
            

            Console.WriteLine("Hello World!");
        }
    }
}