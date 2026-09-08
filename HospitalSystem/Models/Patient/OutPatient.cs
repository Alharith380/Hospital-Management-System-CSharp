using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HospitalSystem.Models
{
    public  class OutPatient:Patient
    {
        public override bool CanBeDischarged()
        {
            // الخارجي لا يتم "إخراجه" بالمعنى الإداري، هو يغادر فوراً بعد العلاج
            return false;
        }

        public override bool CanBeAdmittedTo(string departmentName)
        {
            // الخارجي قد يقبل في عيادات خارجية أو أجهزة خارجية
            return true;
        }

        public override string ToFileString()
        {
            // الخارجي لا يملك تاريخ خروج ولا قسم إقامة دائم
            return $"OUTPATIENT|{Id}|{Name}|{Address}|{DateOfBirth:yyyy-MM-dd}|NULL|NULL";
        }
        public static OutPatient FromFileString(string line)
        {
            string[] parts = line.Split('|');
            // تنسيق السطر المتوقع: OUTPATIENT|Id|Name|Address|DOB|NULL|NULL
            if (parts.Length < 7) throw new FormatException("بيانات مريض خارجي غير صحيحة.");

            return new OutPatient
            {
                Id = int.Parse(parts[1]),
                Name = parts[2],
                Address = parts[3],
                DateOfBirth = DateTime.Parse(parts[4])
                // لا يوجد قسم دائم ولا تاريخ خروج للمريض الخارجي
            };
        }
    }
}
