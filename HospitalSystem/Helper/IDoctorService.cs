using HospitalSystem.Models;
using System.Collections.Generic;

namespace HospitalSystem.Interfaces
{
    public interface IDoctorService
    {
        void RegisterDoctor(Doctor doctor);
        Doctor GetDoctorById(int id);
        List<Doctor> GetAllDoctors();
        int GetResidentsCount();
        decimal CalculateDoctorShare(Treatment treatment, Doctor doctor);
        List<Treatment> GetDoctorHistory(int doctorId);
        void UpdateDoctor(Doctor doctor);
        void DeleteDoctor(int id);
    }
}