using HospitalSystem.Helper;
using HospitalSystem.Interfaces;
using HospitalSystem.Models;
using HospitalSystem.Repositories;
using HospitalSystem.Services;
using HospitalSystem.Exceptions;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace HospitalSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Ensure correct decimal reading/writing (especially for Arabic systems)
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // -----------------------------
            // Dependency Injection Setup
            // -----------------------------
            IRepository<Doctor> doctorRepo;
            IRepository<Patient> patientRepo;
            IRepository<Treatment> treatmentRepo;

            try
            {
                doctorRepo = new DoctorRepository();
                patientRepo = new PatientRepository();
                treatmentRepo = new TreatmentRepository();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize repositories.");
                PrintException(ex);
                Pause();
                return;
            }

            // Services
            IDoctorService doctorService = new DoctorService(doctorRepo, treatmentRepo);
            IPatientService patientService = new PatientService(patientRepo, treatmentRepo);
            ITreatmentService treatmentService = new TreatmentService(treatmentRepo, patientRepo, doctorRepo, doctorService);

            IReportService reportService = new ReportService(
                doctorService,
                patientService,
                treatmentService,
                doctorRepo,
                patientRepo
            );

            // -----------------------------
            // Main Loop
            // -----------------------------
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("Hospital Management System");
                Console.WriteLine("=========================================");
                Console.WriteLine("1) Add Doctor");
                Console.WriteLine("2) List All Doctors");
                Console.WriteLine("3) Delete Doctor");
                Console.WriteLine("4) Count Resident Doctors");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("5) Add Patient (Inpatient/Outpatient)");
                Console.WriteLine("6) List All Patients");
                Console.WriteLine("7) Admit Patient to Department");
                Console.WriteLine("8) Discharge Patient (Inpatient only)");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("9) Record Treatment");
                Console.WriteLine("10) Doctor Report");
                Console.WriteLine("11) Patient Report");
                Console.WriteLine("12) Department Statistics (Date Range)");
                Console.WriteLine("13) Patient Treatments (Date Range)");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("0) Exit");
                Console.WriteLine("=========================================");

                int choice = ReadInt("Enter your choice: ");

                try
                {
                    switch (choice)
                    {
                        case 1:
                            AddDoctor(doctorService);
                            break;
                        case 2:
                            ListDoctors(doctorService);
                            break;
                        case 3:
                            DeleteDoctor(doctorService);
                            break;
                        case 4:
                            ShowResidentsCount(doctorService);
                            break;

                        case 5:
                            AddPatient(patientService);
                            break;
                        case 6:
                            ListPatients(patientService);
                            break;
                        case 7:
                            AdmitPatient(patientService);
                            break;
                        case 8:
                            DischargePatient(patientService);
                            break;

                        case 9:
                            PerformTreatment(treatmentService);
                            break;

                        case 10:
                            PrintDoctorReport(reportService);
                            break;
                        case 11:
                            PrintPatientReport(reportService);
                            break;
                        case 12:
                            PrintDepartmentStatistics(reportService);
                            break;
                        case 13:
                            PrintPatientTreatmentsByDate(reportService);
                            break;

                        case 0:
                            Console.WriteLine("Goodbye.");
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (HospitalException hex)
                {
                    Console.WriteLine("\nHospital system error:");
                    Console.WriteLine(hex.Message);
                    if (hex.InnerException != null)
                    {
                        Console.WriteLine("Additional details:");
                        Console.WriteLine(hex.InnerException.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nUnexpected error:");
                    PrintException(ex);
                }

                Pause();
            }
        }

        // -----------------------------
        // Doctors
        // -----------------------------
        static void AddDoctor(IDoctorService doctorService)
        {
            Console.WriteLine("\n=== Add Doctor ===");
            Console.WriteLine("Doctor types:");
            Console.WriteLine("1) Resident (Trainee)");
            Console.WriteLine("2) Contractor");
            Console.WriteLine("3) Faculty");

            int type = ReadInt("Choose type: ");

            string name = ReadString("Doctor name: ");
            string address = ReadString("Address: ");
            DateTime dob = ReadDate("Date of birth (yyyy-MM-dd): ");
            DateTime startDate = ReadDate("Start date (yyyy-MM-dd): ");
            decimal baseSalary = ReadDecimal("Base salary: ");

            Doctor doctor;

            switch (type)
            {
                case 1:
                    DateTime? endDate = ReadOptionalDate("Training end date (yyyy-MM-dd) or leave blank: ");
                    doctor = new ResidentDoctor
                    {
                        Name = name,
                        Address = address,
                        DateOfBirth = dob,
                        StartDate = startDate,
                        BaseSalary = baseSalary,
                        EndDate = endDate
                    };
                    break;

                case 2:
                    doctor = new ContractorDoctor
                    {
                        Name = name,
                        Address = address,
                        DateOfBirth = dob,
                        StartDate = startDate,
                        BaseSalary = baseSalary
                    };
                    break;

                case 3:
                    doctor = (Doctor)Activator.CreateInstance(typeof(FacultyDoctor));
                    doctor.Name = name;
                    doctor.Address = address;
                    doctor.DateOfBirth = dob;
                    doctor.StartDate = startDate;
                    doctor.BaseSalary = baseSalary;
                    break;

                default:
                    Console.WriteLine("Invalid type.");
                    return;
            }

            doctorService.RegisterDoctor(doctor);
        }

        static void ListDoctors(IDoctorService doctorService)
        {
            Console.WriteLine("\n=== All Doctors ===");
            var doctors = doctorService.GetAllDoctors();

            if (doctors == null || doctors.Count == 0)
            {
                Console.WriteLine("No doctors registered.");
                return;
            }

            foreach (var d in doctors.OrderBy(d => d.Id))
            {
                Console.WriteLine($"- ID={d.Id} | {d.Name} | Type={d.GetType().Name} | Start={d.StartDate:yyyy-MM-dd} | Salary={d.BaseSalary}");
            }
        }

        static void DeleteDoctor(IDoctorService doctorService)
        {
            Console.WriteLine("\n=== Delete Doctor ===");
            int id = ReadInt("Doctor ID: ");
            doctorService.DeleteDoctor(id);
            Console.WriteLine("Doctor deleted (if existed).");
        }

        static void ShowResidentsCount(IDoctorService doctorService)
        {
            Console.WriteLine("\n=== Resident Doctors Count ===");
            int count = doctorService.GetResidentsCount();
            Console.WriteLine($"Number of Residents: {count}");
        }

        // -----------------------------
        // Patients
        // -----------------------------
        static void AddPatient(IPatientService patientService)
        {
            Console.WriteLine("\n=== Add Patient ===");
            Console.WriteLine("1) Inpatient");
            Console.WriteLine("2) Outpatient");
            int type = ReadInt("Choose type: ");

            string name = ReadString("Patient name: ");
            string address = ReadString("Address: ");
            DateTime dob = ReadDate("Date of birth (yyyy-MM-dd): ");

            Patient patient;
            if (type == 1)
            {
                string dept = ReadString("Current department (e.g., Emergency/Eye/Skin): ");
                patient = new Inpatient
                {
                    Name = name,
                    Address = address,
                    DateOfBirth = dob,
                    CurrentDepartment = dept,
                    DischargeDate = null
                };
            }
            else if (type == 2)
            {
                patient = new OutPatient
                {
                    Name = name,
                    Address = address,
                    DateOfBirth = dob
                };
            }
            else
            {
                Console.WriteLine("Invalid type.");
                return;
            }

            patientService.RegisterPatient(patient);
        }

        static void ListPatients(IPatientService patientService)
        {
            Console.WriteLine("\n=== All Patients ===");
            var patients = patientService.GetAllPatients();
            if (patients == null || patients.Count == 0)
            {
                Console.WriteLine("No patients registered.");
                return;
            }

            foreach (var p in patients.OrderBy(p => p.Id))
            {
                string extra = "";
                if (p is Inpatient inp)
                {
                    extra = $" | Dept={(inp.CurrentDepartment ?? "NULL")} | Discharge={(inp.DischargeDate?.ToString("yyyy-MM-dd") ?? "NULL")}";
                }
                Console.WriteLine($"- ID={p.Id} | {p.Name} | Type={p.GetType().Name} | DOB={p.DateOfBirth:yyyy-MM-dd}{extra}");
            }
        }

        static void AdmitPatient(IPatientService patientService)
        {
            Console.WriteLine("\n=== Admit Patient to Department ===");
            int patientId = ReadInt("Patient ID: ");
            string dept = ReadString("Department name: ");
            patientService.AdmitToDepartment(patientId, dept);
        }

        static void DischargePatient(IPatientService patientService)
        {
            Console.WriteLine("\n=== Discharge Patient ===");
            int patientId = ReadInt("Patient ID: ");
            patientService.DischargePatient(patientId);
        }

        // -----------------------------
        // Treatments
        // -----------------------------
        static void PerformTreatment(ITreatmentService treatmentService)
        {
            Console.WriteLine("\n=== Record Treatment ===");
            int patientId = ReadInt("Patient ID: ");
            int doctorId = ReadInt("Doctor ID: ");
            string dept = ReadString("Department name: ");
            decimal cost = ReadDecimal("Treatment cost: ");

            treatmentService.PerformTreatment(patientId, doctorId, dept, cost);
        }

        // -----------------------------
        // Reports
        // -----------------------------
        static void PrintDoctorReport(IReportService reportService)
        {
            Console.WriteLine("\n=== Doctor Report ===");
            int doctorId = ReadInt("Doctor ID: ");
            reportService.PrintDoctorReport(doctorId);
        }

        static void PrintPatientReport(IReportService reportService)
        {
            Console.WriteLine("\n=== Patient Report ===");
            int patientId = ReadInt("Patient ID: ");
            reportService.PrintPatientReport(patientId);
        }

        static void PrintDepartmentStatistics(IReportService reportService)
        {
            Console.WriteLine("\n=== Department Statistics (Date Range) ===");
            string dept = ReadString("Department name: ");
            DateTime from = ReadDate("From date (yyyy-MM-dd): ");
            DateTime to = ReadDate("To date (yyyy-MM-dd): ");
            reportService.PrintDepartmentStatistics(dept, from, to);
        }

        static void PrintPatientTreatmentsByDate(IReportService reportService)
        {
            Console.WriteLine("\n=== Patient Treatments (Date Range) ===");
            int patientId = ReadInt("Patient ID: ");
            DateTime from = ReadDate("From date (yyyy-MM-dd): ");
            DateTime to = ReadDate("To date (yyyy-MM-dd): ");
            reportService.PrintPatientTreatmentsByDate(patientId, from, to);
        }

        // -----------------------------
        // Helpers
        // -----------------------------
        static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void PrintException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (int.TryParse(s, out int v)) return v;
                Console.WriteLine("Please enter a valid integer.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v)) return v;
                if (decimal.TryParse(s, out v)) return v;
                Console.WriteLine("Please enter a valid decimal number.");
            }
        }

        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Value cannot be empty.");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();

                if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    return d;

                if (DateTime.TryParse(s, out d))
                    return d;

                Console.WriteLine("Invalid date format. Example: 2024-04-12");
            }
        }

        static DateTime? ReadOptionalDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(s))
                    return null;

                if (DateTime.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    return d;

                if (DateTime.TryParse(s, out d))
                    return d;

                Console.WriteLine("Invalid date format. Example: 2024-04-12 or leave blank.");
            }
        }
    }
}

