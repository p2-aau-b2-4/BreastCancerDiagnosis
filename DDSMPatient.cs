using System;
using System.Collections.Generic;
using DICOMTests;

namespace DicomDisplayTest
{
    public class DDSMPatient
    {
        public List<DdsmImage> Images;
        public String PatientId {get;set;}
    }
}