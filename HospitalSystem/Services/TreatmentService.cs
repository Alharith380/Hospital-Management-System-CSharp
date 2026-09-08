
using HospitalSystem.Models;
using HospitalSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalSystem.Helper;
using HospitalSystem.Interfaces;

namespace HospitalSystem.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly IRepository<Treatment> _treatmentRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IDoctorService _doctorService; // نحتاجه لحساب الحصة المالية

        public TreatmentService(
            IRepository<Treatment> treatmentRepository,
            IRepository<Patient> patientRepository,
            IRepository<Doctor> doctorRepository,
            IDoctorService doctorService)
        {
            _treatmentRepository = treatmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _doctorService = doctorService;
        }

        /// <summary>
        /// تنفيذ عملية علاج لمريض بواسطة طبيب معين
        /// "إضافة علاج لمريض" + "تسجيل تاريخ العلاج والتكلفة"
        /// </summary>
        public void PerformTreatment(int patientId, int doctorId, string departmentName, decimal cost)
        {
            // 1. التحقق من وجود الكيانات
            var patient = _patientRepository.GetById(patientId);
            if (patient == null) throw new PatientNotFoundException(patientId);

            var doctor = _doctorRepository.GetById(doctorId);
            if (doctor == null) throw new Exception($"الطبيب برقم {doctorId} غير موجود.");

            // 2. حساب حصة الطبيب المالية بناءً على القواعد (المقيم/المتعاقد)
            // ننشئ كائن علاج مؤقت لحساب النسبة
            var tempTreatment = new Treatment
            {
                Cost = cost,
                Date = DateTime.Now,
                DepartmentName = departmentName
            };

            decimal doctorShare = _doctorService.CalculateDoctorShare(tempTreatment, doctor);

            // 3. إنشاء سجل العلاج النهائي
            var treatment = new Treatment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                DepartmentName = departmentName,
                Cost = cost,
                CalculatedDoctorShare = doctorShare, // حفظ الحصة المحسوبة للأرشيف
                Date = DateTime.Now,
                Id = 0 // سيتم تعيينه في الـ Repository
            };

            // 4. الحفظ في قاعدة البيانات (الملف النصي)
            _treatmentRepository.Add(treatment);

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"✅ تم تسجيل العلاج بنجاح!");
            Console.WriteLine($"   المريض: {patient.Name} | الطبيب: {doctor.Name}");
            Console.WriteLine($"   القسم: {departmentName} | التكلفة الكلية: {cost} $");
            Console.WriteLine($"   💰 حصة الطبيب المالية المحسوبة: {doctorShare} $");
            Console.WriteLine("--------------------------------------------------");
        }

        /// <summary>
        /// جلب جميع العلاجات
        /// </summary>
        public List<Treatment> GetAllTreatments()
        {
            return _treatmentRepository.GetAll();
        }

        /// <summary>
        /// جلب العلاجات ضمن فترة زمنية محددة (مطلوب للتقارير)
        /// "عرض علاجات مريض في فترة زمنية محددة" / "حساب عدد المرضى في قسم خلال فترة"
        /// </summary>
        public List<Treatment> GetTreatmentsByDateRange(DateTime start, DateTime end)
        {
            return _treatmentRepository.GetAll()
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .ToList();
        }

        /// <summary>
        /// جلب علاجات قسم محدد في فترة زمنية
        /// </summary>
        public List<Treatment> GetTreatmentsByDepartment(string deptName, DateTime start, DateTime end)
        {
            return _treatmentRepository.GetAll()
                .Where(t => t.DepartmentName.Equals(deptName, StringComparison.OrdinalIgnoreCase) &&
                            t.Date >= start && t.Date <= end)
                .ToList();
        }

        // دوال مساعدة للتحديث والحذف إذا لزم الأمر حسب متطلبات "تعديل/حذف سجل علاج"
        public void UpdateTreatment(Treatment treatment) => _treatmentRepository.Update(treatment);
        public void DeleteTreatment(int id) => _treatmentRepository.Delete(id);
    }
}