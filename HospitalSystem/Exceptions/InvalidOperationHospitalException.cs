using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    public  class InvalidOperationHospitalException:HospitalException
    {
        public InvalidOperationHospitalException(string message)
            : base(message) { }
    }
}
