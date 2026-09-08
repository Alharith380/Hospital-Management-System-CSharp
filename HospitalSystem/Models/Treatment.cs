using System;

namespace HospitalSystem.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public string DepartmentName { get; set; }
        public decimal Cost { get; set; }

        // حقل اختياري لتخزين حصة الطبيب المحسوبة وقت العلاج (لأغراض الأرشفة)
        public decimal CalculatedDoctorShare { get; set; }

        public Treatment() { }

        public Treatment(int id, int patientId, int doctorId, DateTime date, string dept, decimal cost, decimal share = 0)
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            Date = date;
            DepartmentName = dept;
            Cost = cost;
            CalculatedDoctorShare = share;
        }

        // تحويل لسطر ملف: ID|PatientID|DoctorID|Date|Dept|Cost|Share
        public string ToFileString()
        {
            return $"{Id}|{PatientId}|{DoctorId}|{Date:yyyy-MM-dd}|{DepartmentName}|{Cost}|{CalculatedDoctorShare}";
        }

        // قراءة من سطر ملف
        public static Treatment FromFileString(string line)
        {
            string[] parts = line.Split('|');
            if (parts.Length < 6) throw new FormatException("بيانات العلاج غير مكتملة.");

            // نتعامل مع الحصة الاختيارية بحذر في حال كانت الملفات القديمة لا تحتويها
            decimal share = 0;
            if (parts.Length >= 7) decimal.TryParse(parts[6], out share);

            return new Treatment
            {
                Id = int.Parse(parts[0]),
                PatientId = int.Parse(parts[1]),
                DoctorId = int.Parse(parts[2]),
                Date = DateTime.Parse(parts[3]),
                DepartmentName = parts[4],
                Cost = decimal.Parse(parts[5]),
                CalculatedDoctorShare = share
            };
        }
    }
}