using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    public class InvalidDepartmentAssignmentException:HospitalException
    {
      
            public InvalidDepartmentAssignmentException(string message)
                : base(message) { }
        }
    }

