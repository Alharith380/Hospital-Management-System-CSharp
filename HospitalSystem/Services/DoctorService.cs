using HospitalSystem.Helper;
using HospitalSystem.Interfaces;
using HospitalSystem.Models;
 // للوصول إلى ResidentDoctor, ContractorDoctor
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalSystem.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Treatment> _treatmentRepository;

        // حقن التبعيات (Dependency Injection) يدوياً
        public DoctorService(IRepository<Doctor> doctorRepository, IRepository<Treatment> treatmentRepository)
        {
            _doctorRepository = doctorRepository;
            _treatmentRepository = treatmentRepository;
        }

        /// <summary>
        /// تسجيل طبيب جديد (إضافة للسجل)
        /// </summary>
        public void RegisterDoctor(Doctor doctor)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            if (string.IsNullOrWhiteSpace(doctor.Name))
                throw new ArgumentException("اسم الطبيب مطلوب ولا يمكن أن يكون فارغاً.");

            // التحقق من تاريخ البدء للمقيمين (منطق وقائي)
            if (doctor is ResidentDoctor resident && resident.StartDate > DateTime.Now)
                throw new ArgumentException("تاريخ بدء التدريب لا يمكن أن يكون في المستقبل.");

            _doctorRepository.Add(doctor);
            Console.WriteLine($"تم تسجيل الطبيب {doctor.Name} بنجاح.");
        }

        /// <summary>
        /// جلب طبيب حسب المعرف
        /// </summary>
        public Doctor GetDoctorById(int id)
        {
            return _doctorRepository.GetById(id);
        }

        /// <summary>
        /// جلب قائمة جميع الأطباء
        /// </summary>
        public List<Doctor> GetAllDoctors()
        {
            return _doctorRepository.GetAll();
        }

        /// <summary>
        /// حساب عدد الأطباء المقيمين فقط (متطلب من الوظيفة)
        /// </summary>
        public int GetResidentsCount()
        {
            // تأكد من وجود () بعد Count
            return _doctorRepository.GetAll()
                .Where(d => d is ResidentDoctor)
                .Count(); // <--- تأكد من وجود الأقواس هنا
        }

        /// <summary>
        /// حساب الحصة المالية للطبيب بناءً على نوعه وخبرته (المنطق الأهم في الوظيفة)
        /// </summary>
        public decimal CalculateDoctorShare(Treatment treatment, Doctor doctor)
        {
            if (treatment == null || doctor == null) return 0;

            // الحالة 1: طبيب متعاقد
            // النص: "يتقاضى 50% من تكاليف العلاج التي يجريها"
            if (doctor is ContractorDoctor)
            {
                return treatment.Cost * 0.50m;
            }

            // الحالة 2: طبيب مقيم
            // النص: "يزداد راتبه... 50% إذا ثبت سنتين... وفي السنة الثانية 75%"
            // التفسير: 
            // - أقل من سنتين: 0% حافز (راتبه الثابت فقط).
            // - بعد إكمال سنتين (السنة الثالثة فصاعداً): 50%.
            // - ملاحظة: النص يقول "وفي السنة الثانية 75%" وهذا قد يعني تراكمي أو مرحلة محددة.
            // سنعتمد التفسير المنطقي الأكاديمي الشائع: 
            // < 2 سنوات = 0%
            // >= 2 سنوات و < 3 سنوات = 50%
            // >= 3 سنوات (أو ما فسرته الوظيفة بالسنة الثانية المتقدمة) = 75%
            // *سنطبق منطقاً مرناً يغطي النص حرفياً:*

            if (doctor is ResidentDoctor resident)
            {
                int yearsOfExperience = resident.GetYearsOfExperience();

                if (yearsOfExperience < 2)
                {
                    // لم يكمل سنتين بعد: لا حصة من العلاج (فقط راتب ثابت يزداد لكن لا يؤثر على تكلفة الجلسة مباشرة هنا)
                    return 0;
                }
                else if (yearsOfExperience == 2)
                {
                    // أكمل سنتين بالضبط: يأخذ 50%
                    return treatment.Cost * 0.50m;
                }
                else
                {
                    // بعد السنتين (السنة الثانية فما فوق حسب نص "75%"): يأخذ 75%
                    // ملاحظة: إذا كان المقصود بالسنة الثانية هي السنة رقم 2 تماماً، عدل الشرط أعلاه.
                    // لكن غالباً "السنة الثانية" تعني المرحلة التالية بعد الإثبات.
                    return treatment.Cost * 0.75m;
                }
            }

            // الحالة 3: عضو هيئة تدريس
            // النص لم يذكر نسبة محددة لهم من العلاج، نفترض أنهم براتب ثابت أو النسبة 0 في هذا السياق
            if (doctor is FacultyDoctor)
            {
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// جلب سجل علاجات طبيب معين مرتبة تسلسلياً (متطلب من الوظيفة)
        /// "إنشاء سلسلة مرتبة لسجلات الأطباء"
        /// </summary>
        public List<Treatment> GetDoctorHistory(int doctorId)
        {
            var allTreatments = _treatmentRepository.GetAll();

            // فلترة العلاجات الخاصة بهذا الطبيب وترتيبها حسب التاريخ
            return allTreatments
                .Where(t => t.DoctorId == doctorId)
                .OrderBy(t => t.Date)
                .ToList();
        }

        public void UpdateDoctor(Doctor doctor)
        {
            _doctorRepository.Update(doctor);
        }

        public void DeleteDoctor(int id)
        {
            _doctorRepository.Delete(id);
        }
    }
}