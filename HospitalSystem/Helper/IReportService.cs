using System;

namespace HospitalSystem.Interfaces
{
    public interface IReportService
    {
        void PrintDoctorReport(int doctorId);
        void PrintPatientReport(int patientId);
        void PrintDepartmentStatistics(string departmentName, DateTime from, DateTime to);
        void PrintPatientTreatmentsByDate(int patientId, DateTime from, DateTime to);
    }
}