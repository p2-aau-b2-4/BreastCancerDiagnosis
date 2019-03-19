using System;
using System.Collections.Generic;

namespace DicomDisplayTest
{
    public class DDSMPatient
    {
        public List<DDSMImage> Images;
        public String PatientId {get;set;}
    }
}