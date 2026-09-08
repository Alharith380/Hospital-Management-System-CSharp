using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Models
{
    public abstract class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal BaseSalary { get; set; } // الراتب الأساسي
        public DateTime StartDate { get; set; }

        // دالة مجردة: كل نوع طبيب سيحسب حصته بطريقته الخاصة
        // هذا هو جوهر الـ Polymorphism
        public abstract decimal CalculateShare(decimal treatmentCost);

        // دالة مساعدة لحساب سنوات الخبرة
        public int GetYearsOfExperience()
        {
            return (DateTime.Now - StartDate).Days / 365;
        }
        public abstract string ToFileString();
        public static Doctor FromFileString(string line)
        {
            // منطق القراءة سيتطلب معرفة النوع أولاً لإنشاء الكلاس الصحيح
            // سنناقش تفاصيل التنفيذ في قسم Repository
            throw new NotImplementedException("يتم التعامل مع هذه الدالة في Factory Method");
        }
    }
    }
