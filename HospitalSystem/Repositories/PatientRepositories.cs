
using HospitalSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalSystem.Helper;
using HospitalSystem.Models;

namespace HospitalSystem.Repositories
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly string _filePath = "patients.txt";
        private List<Patient> _patients;

        public PatientRepository()
        {
            _patients = new List<Patient>();
            LoadData();
        }

        public void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                _patients = new List<Patient>();
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_filePath);
                _patients = new List<Patient>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string typeIdentifier = line.Split('|')[0];

                    Patient patient = typeIdentifier switch
                    {
                        "INPATIENT" => Inpatient.FromFileString(line),
                        "OUTPATIENT" => OutPatient.FromFileString(line),
                        _ => throw new UnknownEntityTypeException(typeIdentifier)
                    };

                    _patients.Add(patient);
                }
            }
            catch (Exception ex)
            {
                throw new DataPersistenceException(_filePath, ex);
            }
        }

        public List<Patient> GetAll() => _patients;

        public Patient GetById(int id)
        {
            return _patients.FirstOrDefault(p => p.Id == id)
                ?? throw new PatientNotFoundException(id);
        }

        public void Add(Patient patient)
        {
            if (_patients.Count > 0)
                patient.Id = _patients.Max(p => p.Id) + 1;
            else
                patient.Id = 1;

            _patients.Add(patient);
            SaveChanges();
        }

        public void Update(Patient patient)
        {
            var existing = _patients.FirstOrDefault(p => p.Id == patient.Id);
            if (existing != null)
            {
                _patients.Remove(existing);
                _patients.Add(patient);
                SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient != null)
            {
                _patients.Remove(patient);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            try
            {
                var lines = new List<string>();
                foreach (var pat in _patients)
                {
                    lines.Add(pat.ToFileString());
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