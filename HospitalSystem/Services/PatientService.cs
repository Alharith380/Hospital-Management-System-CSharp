
using HospitalSystem.Models;
 // للوصول إلى Inpatient, Outpatient
using HospitalSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalSystem.Helper;
using HospitalSystem.Interfaces;

namespace HospitalSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Treatment> _treatmentRepository;

        public PatientService(IRepository<Patient> patientRepository, IRepository<Treatment> treatmentRepository)
        {
            _patientRepository = patientRepository;
            _treatmentRepository = treatmentRepository;
        }

        /// <summary>
        /// تسجيل مريض جديد
        /// </summary>
        public void RegisterPatient(Patient patient)
        {
            if (patient == null) throw new ArgumentNullException(nameof(patient));
            if (string.IsNullOrWhiteSpace(patient.Name))
                throw new ArgumentException("اسم المريض مطلوب.");

            _patientRepository.Add(patient);
            Console.WriteLine($"تم تسجيل المريض {patient.Name} (نوع السجل: {patient.GetType().Name}) بنجاح.");
        }

        public Patient GetPatientById(int id) => _patientRepository.GetById(id);
        public List<Patient> GetAllPatients() => _patientRepository.GetAll();


        /// <summary>
        /// قبول مريض في قسم معين (تطبيق قواعد الوظيفة الصارمة)
        /// </summary>
        public void AdmitToDepartment(int patientId, string departmentName)
        {
            var patient = _patientRepository.GetById(patientId);

            if (patient == null) throw new PatientNotFoundException(patientId);

            // التحقق من قاعدة الوظيفة:
            // "السجل الخارجي يؤدي على قبوله في العيادات الخارجية أو الأجهزة الخارجية"
            // سنفترض أن "الأجهزة الخارجية" و "العيادات" هي الأقسام المسموحة للخارجي.
            // بينما "العنبر" أو "الإقامة الداخلية" ممنوعة عليه.

            bool isExternalDept = departmentName.Contains("خارجية") ||
                                  departmentName.Contains("عيادات") ||
                                  departmentName.Contains("أجهزة");

            if (patient is OutPatient)
            {
                if (!isExternalDept)
                {
                    throw new InvalidDepartmentAssignmentException(
                        $"خطأ: لا يمكن قبول مريض خارجي في قسم '{departmentName}'. \nالقاعدة: يقبل الخارجي فقط في العيادات الخارجية أو الأجهزة الخارجية.");
                }
                Console.WriteLine($"تم قبول المريض الخارجي {patient.Name} للزيارة في قسم {departmentName}.");
            }
            else if (patient is Inpatient inpatient)
            {
                // المريض الداخلي يقبل في الأقسام الداخلية للإقامة
                inpatient.CurrentDepartment = departmentName;
                // إذا كان المريض قد خرج سابقاً وأعيد إدخاله، نصفر تاريخ الخروج
                if (inpatient.DischargeDate.HasValue)
                {
                    inpatient.DischargeDate = null;
                }
                _patientRepository.Update(inpatient);
                Console.WriteLine($"تم إدخال المريض الداخلي {patient.Name} إلى قسم {departmentName}.");
            }
        }

        /// <summary>
        /// إخراج مريض داخلي وتسجيل تاريخ الخروج
        /// "تسجيل تاريخ خروج المريض من القسم الداخلي"
        /// </summary>
        public void DischargePatient(int patientId)
        {
            var patient = _patientRepository.GetById(patientId);

            if (patient == null) throw new PatientNotFoundException(patientId);

            // القاعدة: لا يمكن إخراج مريض خارجي (هو يغادر فوراً بعد الزيارة)
            if (patient is OutPatient)
            {
                throw new InvalidOperationHospitalException(
                    "لا يمكن تنفيذ إجراء 'إخراج' على مريض خارجي. سجلاته تنتهي بانتهاء الزيارة.");
            }

            if (patient is Inpatient inpatient)
            {
                // التحقق: هل تم إخراجه مسبقاً؟
                if (inpatient.DischargeDate.HasValue)
                {
                    throw new InvalidOperationHospitalException(
                        $"المريض {patient.Name} تم إخراجه مسبقاً بتاريخ {inpatient.DischargeDate.Value:yyyy-MM-dd}.");
                }

                // تنفيذ الإخراج: تسجيل التاريخ الحالي
                inpatient.DischargeDate = DateTime.Now;
                inpatient.CurrentDepartment = null; // تحرير القسم

                _patientRepository.Update(inpatient);
                Console.WriteLine($"تم إخراج المريض الداخلي {patient.Name} بنجاح. تاريخ الخروج: {inpatient.DischargeDate.Value}");
            }
        }

        /// <summary>
        /// جلب سجل علاجات مريض معين مرتبة تسلسلياً
        /// </summary>
        public List<Treatment> GetPatientHistory(int patientId)
        {
            var allTreatments = _treatmentRepository.GetAll();
            return allTreatments
                .Where(t => t.PatientId == patientId)
                .OrderBy(t => t.Date)
                .ToList();
        }

        public void UpdatePatient(Patient patient) => _patientRepository.Update(patient);
    }
}

