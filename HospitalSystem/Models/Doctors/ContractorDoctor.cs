using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Models
{
    public class ContractorDoctor:Doctor
    {
        public ContractorDoctor()
        {
            BaseSalary = 0; // المتعاقد يعتمد على النسبة فقط غالباً
        }

        public override decimal CalculateShare(decimal treatmentCost)
        {
            // القاعدة: 50% من تكاليف العلاج دائماً
            return treatmentCost * 0.50m;
        }

        public override string ToFileString()
        {
            // نضيف علامة "CONTRACTOR"
            return $"CONTRACTOR|{Id}|{Name}|{Address}|{DateOfBirth:yyyy-MM-dd}|{BaseSalary}|{StartDate:yyyy-MM-dd}|NULL";
        }
    }
}
