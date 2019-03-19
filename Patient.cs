using System;
using System.Collections.Generic;

namespace DicomDisplayTest
{
    public class Patient
    {
        public int Age { get; }
        public string PatientId { get; }
        public List<Mammography> mammographies { get; set; }
        
        public static List<Patient> GetAllPatientsFromDirectory(String directory)
        {
            throw new NotImplementedException();
        }

        
        
    }


}