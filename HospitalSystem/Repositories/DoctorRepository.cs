 // تأكد من الـ Namespace الصحيح
using HospitalSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalSystem.Helper;
using HospitalSystem.Models;

namespace HospitalSystem.Repositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private readonly string _filePath = "doctors.txt";
        private List<Doctor> _doctors;

        public DoctorRepository()
        {
            _doctors = new List<Doctor>();
            LoadData();
        }

        public void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                _doctors = new List<Doctor>();
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_filePath);
                _doctors = new List<Doctor>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // هنا السحر: نقرأ النوع من بداية السطر
                    string typeIdentifier = line.Split('|')[0];

                    Doctor doctor = typeIdentifier switch
                    {
                        "RESIDENT" => ResidentDoctor.FromFileString(line),
                        "CONTRACTOR" => ContractorDoctor.FromFileString(line),
                        "FACULTY" => FacultyDoctor.FromFileString(line),
                        _ => throw new UnknownEntityTypeException(typeIdentifier)
                    };

                    _doctors.Add(doctor);
                }
            }
            catch (Exception ex)
            {
                throw new DataPersistenceException(_filePath, ex);
            }
        }

        public List<Doctor> GetAll() => _doctors;

        public Doctor GetById(int id)
        {
            return _doctors.FirstOrDefault(d => d.Id == id)
                ?? throw new Exception($"طبيب برقم {id} غير موجود");
        }

        public void Add(Doctor doctor)
        {
            // توليد ID تلقائي بسيط
            if (_doctors.Count > 0)
                doctor.Id = _doctors.Max(d => d.Id) + 1;
            else
                doctor.Id = 1;

            _doctors.Add(doctor);
            SaveChanges(); // حفظ فوري للتبسيط، أو يمكن تأجيله
        }

        public void Update(Doctor doctor)
        {
            var existing = _doctors.FirstOrDefault(d => d.Id == doctor.Id);
            if (existing != null)
            {
                // نستبدل العنصر القديم بالجديد
                _doctors.Remove(existing);
                _doctors.Add(doctor);
                SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == id);
            if (doctor != null)
            {
                _doctors.Remove(doctor);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            try
            {
                var lines = new List<string>();
                foreach (var doc in _doctors)
                {
                    // كل كلاس مشتق يعرف كيف يحول نفسه لنص
                    lines.Add(doc.ToFileString());
                }
                File.WriteAllLines(_filePath, lines);
            }
            catch (Exception ex)
            {
                throw new DataPersistenceException(_filePath, ex);
            }
        }
    }
}