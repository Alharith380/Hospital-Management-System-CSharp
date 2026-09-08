using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    public class HospitalException : Exception
    {
        public HospitalException(string message) : base (message){}
        public HospitalException(string message, Exception innerException)
           : base(message, innerException) { }



    }
}
