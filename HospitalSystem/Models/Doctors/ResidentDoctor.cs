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
    public  class ResidentDoctor:Doctor
    {
        public DateTime? EndDate { get; set; } // تاريخ انتهاء الإقامة

        public override decimal CalculateShare(decimal treatmentCost)
        {
            int years = GetYearsOfExperience();

            
            if (years < 2) return 0;

            // بعد سنتين: 50% من تكلفة العلاج
            // ملاحظة: النص ذكر زيادة الراتب ثم 75% في السنة الثانية، سنطبق المنطق المذكور في الشرح السابق (50%) للتبسيط أو يمكن تعديله حسب الدقة المطلوبة
            // بناءً على النص: "يزداد راتبه... 50% ... إذا ثبت سنتين"
            return treatmentCost * 0.50m;
        }

        public override string ToFileString()
        {
            string endStr = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : "NULL";
            // نضيف علامة "RESIDENT" لنعرف نوعه عند القراءة
            return $"RESIDENT|{Id}|{Name}|{Address}|{DateOfBirth:yyyy-MM-dd}|{BaseSalary}|{StartDate:yyyy-MM-dd}|{endStr}";
        }
    }
}
