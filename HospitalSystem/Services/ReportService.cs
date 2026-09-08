using HospitalSystem.Helper;
using HospitalSystem.Interfaces;
using HospitalSystem.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly ITreatmentService _treatmentService;
        private readonly IRepository<Doctor> _doctorRepo;
        private readonly IRepository<Patient> _patientRepo;

        public ReportService(
            IDoctorService doctorService,
            IPatientService patientService,
            ITreatmentService treatmentService,
            IRepository<Doctor> doctorRepo,
            IRepository<Patient> patientRepo)
        {
            _doctorService = doctorService;
            _patientService = patientService;
            _treatmentService = treatmentService;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
        }

        /// <summary>
        /// "إنشاء سلسلة مرتبة لسجلات الأطباء"
        /// عرض سجلات طبيب معين مرتبة تسلسلياً حسب التاريخ
        /// </summary>
        public void PrintDoctorReport(int doctorId)
        {
            var doctor = _doctorRepo.GetById(doctorId);
            if (doctor == null)
            {
                Console.WriteLine("❌ الطبيب غير موجود.");
                return;
            }

            var treatments = _doctorService.GetDoctorHistory(doctorId);

            Console.WriteLine($"\n📋 === تقرير سجلات الطبيب: {doctor.Name} ({doctor.GetType().Name}) ===");
            Console.WriteLine($"الرصيد/الراتب الأساسي: {doctor.BaseSalary} | تاريخ البدء: {doctor.StartDate:yyyy-MM-dd}");

            if (!treatments.Any())
            {
                Console.WriteLine("   لا توجد سجلات علاجية مسجلة لهذا الطبيب حتى الآن.");
                return;
            }

            Console.WriteLine($"{"",-5} {"التاريخ",-12} {"القسم",-20} {"التكلفة",-10} {"حصة الطبيب",-12}");
            Console.WriteLine(new string('-', 65));

            foreach (var t in treatments)
            {
                Console.WriteLine($"{t.Id,-5} {t.Date:yyyy-MM-dd,-12} {t.DepartmentName,-20} {t.Cost,-10:C} {t.CalculatedDoctorShare,-12:C}");
            }
            Console.WriteLine(new string('-', 65));
        }

        /// <summary>
        /// "إنشاء سلسلة مرتبة لسجلات المرضى" + "عرض جميع علاجات مريض"
        /// </summary>
        public void PrintPatientReport(int patientId)
        {
            var patient = _patientRepo.GetById(patientId);
            if (patient == null)
            {
                Console.WriteLine("❌ المريض غير موجود.");
                return;
            }

            var treatments = _patientService.GetPatientHistory(patientId);
            string patientType = patient is Inpatient ? "داخلي" : "خارجي";

            Console.WriteLine($"\n📋 === تقرير سجلات المريض: {patient.Name} (نوع السجل: {patientType}) ===");
            Console.WriteLine($"العنوان: {patient.Address} | تاريخ الميلاد: {patient.DateOfBirth:yyyy-MM-dd}");

            if (patient is Inpatient inp)
            {
                string status = inp.DischargeDate.HasValue ? $"تم الخروج بتاريخ: {inp.DischargeDate.Value:yyyy-MM-dd}" : $"مقيم حالياً في: {inp.CurrentDepartment ?? "غير محدد"}";
                Console.WriteLine($"الحالة: {status}");
            }

            if (!treatments.Any())
            {
                Console.WriteLine("   لا توجد سجلات علاجية لهذا المريض.");
                return;
            }

            Console.WriteLine($"{"",-5} {"التاريخ",-12} {"القسم",-20} {"الطبيب ID",-10} {"التكلفة",-10}");
            Console.WriteLine(new string('-', 65));

            foreach (var t in treatments)
            {
                Console.WriteLine($"{t.Id,-5} {t.Date:yyyy-MM-dd,-12} {t.DepartmentName,-20} {t.DoctorId,-10} {t.Cost,-10:C}");
            }
        }

        /// <summary>
        /// "حساب عدد المرضى في قسم خلال فترة زمنية محددة"
        /// وعرض إحصائيات القسم
        /// </summary>
        public void PrintDepartmentStatistics(string departmentName, DateTime from, DateTime to)
        {
            var treatments = _treatmentService.GetTreatmentsByDepartment(departmentName, from, to);

            Console.WriteLine($"\n📊 === إحصائيات قسم: {departmentName} ===");
            Console.WriteLine($"الفترة الزمنية: من {from:yyyy-MM-dd} إلى {to:yyyy-MM-dd}");

            if (!treatments.Any())
            {
                Console.WriteLine("   لا توجد علاجات مسجلة في هذا القسم خلال الفترة المحددة.");
                return;
            }

            int totalPatients = treatments.Select(t => t.PatientId).Distinct().Count();
            int totalSessions = treatments.Count();
            decimal totalRevenue = treatments.Sum(t => t.Cost);
            decimal totalDoctorsPayout = treatments.Sum(t => t.CalculatedDoctorShare);

            Console.WriteLine($"عدد المرضى الفريدين: {totalPatients}");
            Console.WriteLine($"إجمالي جلسات العلاج: {totalSessions}");
            Console.WriteLine($"إجمالي إيرادات القسم: {totalRevenue:C}");
            Console.WriteLine($"إجمالي مستحقات الأطباء: {totalDoctorsPayout:C}");
            Console.WriteLine($"صافي الربح للقسم (تقريبي): {(totalRevenue - totalDoctorsPayout):C}");
        }

        /// <summary>
        /// "عرض علاجات مريض في فترة زمنية محددة"
        /// </summary>
        public void PrintPatientTreatmentsByDate(int patientId, DateTime from, DateTime to)
        {
            var allHistory = _patientService.GetPatientHistory(patientId);
            var filtered = allHistory.Where(t => t.Date >= from && t.Date <= to).ToList();

            Console.WriteLine($"\n📅 علاجات المريض رقم {patientId} بين {from:yyyy-MM-dd} و {to:yyyy-MM-dd}:");
            if (!filtered.Any())
            {
                Console.WriteLine("   لا توجد علاجات في هذه الفترة.");
                return;
            }

            foreach (var t in filtered)
            {
                Console.WriteLine($"- {t.Date:yyyy-MM-dd} | {t.DepartmentName} | {t.Cost:C}");
            }
        }
    }
}