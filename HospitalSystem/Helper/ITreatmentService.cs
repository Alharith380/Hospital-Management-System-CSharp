using HospitalSystem.Models;
using System;
using System.Collections.Generic;

namespace HospitalSystem.Interfaces
{
    public interface ITreatmentService
    {
        void PerformTreatment(int patientId, int doctorId, string departmentName, decimal cost);
        List<Treatment> GetAllTreatments();
        List<Treatment> GetTreatmentsByDateRange(DateTime start, DateTime end);
        List<Treatment> GetTreatmentsByDepartment(string deptName, DateTime start, DateTime end);
        void UpdateTreatment(Treatment treatment);
        void DeleteTreatment(int id);
    }
}