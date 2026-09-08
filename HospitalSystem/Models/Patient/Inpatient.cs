using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Models
{
    public  class Inpatient:Patient
    {
        public DateTime? DischargeDate { get; set; }
        public string CurrentDepartment { get; set; }

        public override bool CanBeDischarged()
        {
            // المريض الداخلي يمكن إخراجه فقط إذا كان موجوداً ولم يخرج بعد
            return DischargeDate == null;
        }

        public override bool CanBeAdmittedTo(string departmentName)
        {
            // الداخلي يقبل في الأقسام الداخلية فقط (مثلاً العيون، الجلد)
            // يمكن إضافة منطق هنا للتحقق من اسم القسم
            return true; // تبسيطاً للكود حالياً
        }

        public override string ToFileString()
        {
            string dischargeStr = DischargeDate.HasValue ? DischargeDate.Value.ToString("yyyy-MM-dd") : "NULL";
            return $"INPATIENT|{Id}|{Name}|{Address}|{DateOfBirth:yyyy-MM-dd}|{CurrentDepartment}|{dischargeStr}";
        }
        public static Inpatient FromFileString(string line)
        {
            string[] parts = line.Split('|');
            // تنسيق السطر المتوقع: INPATIENT|Id|Name|Address|DOB|Dept|DischargeDate
            if (parts.Length < 7) throw new FormatException("بيانات مريض داخلي غير صحيحة.");

            var dischargeDateStr = parts[6];
            DateTime? dischargeDate = (dischargeDateStr == "NULL") ? null : DateTime.Parse(dischargeDateStr);

            return new Inpatient
            {
                Id = int.Parse(parts[1]),
                Name = parts[2],
                Address = parts[3],
                DateOfBirth = DateTime.Parse(parts[4]),
                CurrentDepartment = parts[5],
                DischargeDate = dischargeDate
            };
        }
    }
}
