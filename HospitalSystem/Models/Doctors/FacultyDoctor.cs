using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HospitalSystem.Models
{
    internal class FacultyDoctor:Doctor
    {
        public override decimal CalculateShare(decimal treatmentCost)
        {
            // النص لم يحدد نسبة واضحة لأعضاء الهيئة من العلاج (غالباً راتب ثابت فقط)
            // نفترض 0% إضافية من الجلسة كحالة افتراضية، أو يمكن تغييرها
            return 0;
        }

        public override string ToFileString()
        {
            return $"FACULTY|{Id}|{Name}|{Address}|{DateOfBirth:yyyy-MM-dd}|{BaseSalary}|{StartDate:yyyy-MM-dd}|NULL";
        }
    }
}
