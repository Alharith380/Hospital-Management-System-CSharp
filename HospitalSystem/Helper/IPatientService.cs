using HospitalSystem.Models;

using System.Collections.Generic;

namespace HospitalSystem.Interfaces
{
    public interface IPatientService
    {
        void RegisterPatient(Patient patient);
        Patient GetPatientById(int id);
        List<Patient> GetAllPatients();
        void AdmitToDepartment(int patientId, string departmentName);
        void DischargePatient(int patientId);
        List<Treatment> GetPatientHistory(int patientId);
        void UpdatePatient(Patient patient);
    }
}