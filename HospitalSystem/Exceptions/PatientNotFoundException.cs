using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    internal class PatientNotFoundException:HospitalException
    {
        public PatientNotFoundException(int patientId)
            : base($"Error: Patient with ID {patientId} was not found in the system.")
        { }
    }
}
