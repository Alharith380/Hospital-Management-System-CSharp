using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    internal class DoctorNotQualifiedException:HospitalException
    {
        public DoctorNotQualifiedException(string doctorName, string reason)
           : base($"Error: Dr. {doctorName} is not qualified for this action. Reason: {reason}")
        { }
    }
}
