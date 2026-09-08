
using System;
using System.Collections.Generic;

namespace HospitalSystem.Models
{
    public abstract class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        // قائمة العلاجات مشتركة بين الجميع
        public List<int> TreatmentIds { get; set; } = new List<int>();

        // دالة مجردة للتحقق من إمكانية الإخراج (الداخلي يحتاج تاريخ، الخارجي لا)
        public abstract bool CanBeDischarged();

        // دالة مجردة للتحقق من قبوله في قسم معين
        public abstract bool CanBeAdmittedTo(string departmentName);

        public abstract string ToFileString();
    }
}



