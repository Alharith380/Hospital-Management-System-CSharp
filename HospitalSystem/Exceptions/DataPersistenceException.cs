using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    internal class DataPersistenceException:HospitalException
    {
        public DataPersistenceException(string fileName, Exception innerException)
           : base($"Error: Failed to save/load data from file '{fileName}'.", innerException)
        { }
    }
}
