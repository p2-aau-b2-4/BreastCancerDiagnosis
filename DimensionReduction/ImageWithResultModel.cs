using System;
using ImagePreprocessing;

namespace Training
{
    [Serializable]
    public class ImageWithResultModel
    {
        public UShortArrayAsImage Image { get; set; }
        public double Result { get; set; } //0 is benign, 1 is malignant
    }
}