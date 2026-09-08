using HospitalSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalSystem.Helper;
using HospitalSystem.Models;

namespace HospitalSystem.Repositories
{
    public class TreatmentRepository : IRepository<Treatment>
    {
        private readonly string _filePath = "treatments.txt";
        private List<Treatment> _treatments;

        public TreatmentRepository()
        {
            _treatments = new List<Treatment>();
            LoadData();
        }

        public void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                _treatments = new List<Treatment>();
                return;
            }

            try
            {
                var lines = File.ReadAllLines(_filePath);
                _treatments = new List<Treatment>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string typeIdentifier = line.Split('|')[0];

                    Treatment treatment = typeIdentifier switch
                    {
                        "TREATMENT" => Treatment.FromFileString(line),
                        _ => throw new UnknownEntityTypeException(typeIdentifier)
                    };

                    _treatments.Add(treatment);
                }
            }
            catch (Exception ex)
            {
                throw new DataPersistenceException(_filePath, ex);
            }
        }

        public List<Treatment> GetAll() => _treatments;

        public Treatment GetById(int id)
        {
            return _treatments.FirstOrDefault(t => t.Id == id)
                ?? throw new Exception($"Treatment with ID {id} not found");
        }

        public void Add(Treatment treatment)
        {
            if (_treatments.Count > 0)
                treatment.Id = _treatments.Max(t => t.Id) + 1;
            else
                treatment.Id = 1;

            _treatments.Add(treatment);
            SaveChanges();
        }

        public void Update(Treatment treatment)
        {
            var existing = _treatments.FirstOrDefault(t => t.Id == treatment.Id);
            if (existing != null)
            {
                _treatments.Remove(existing);
                _treatments.Add(treatment);
                SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var treatment = _treatments.FirstOrDefault(t => t.Id == id);
            if (treatment != null)
            {
                _treatments.Remove(treatment);
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            try
            {
                var lines = new List<string>();
                foreach (var t in _treatments)
                {
                    lines.Add(t.ToFileString());
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

