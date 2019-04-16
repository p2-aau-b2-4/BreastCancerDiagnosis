using System;

namespace ImagePreprocessing
{
    public class Mammography
    {
        enum SideEnum {Left,Right}
        public int BreastDensity { get; }
        public int Side { get; }
        public String ImageView { get; }
        
        public ushort[,] GetPixelData()
        {
            throw new NotImplementedException();
        }
    }
}