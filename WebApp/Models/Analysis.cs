using System;
using ImagePreprocessing;

namespace WebApp.Models
{
    public class Analysis
    {
        public Guid Id { get; set; }
        //  public UShortArrayAsImage Image { get; set; }

        public string Status { get; set; }
        public double Certainty { get; set; }

        public DdsmImage.Pathologies Diagnosis { get; set; }

        public int PercentageDone { get; set; }
    }
}