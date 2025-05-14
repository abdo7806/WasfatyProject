using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;


namespace Wasfaty.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Patient> AddAsync(Patient patient)
        {
            
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();
                return patient;
          
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                try
                {
                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;

                }

            }
            return false;
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {

            /*   await _context.Users
                   .Select(user => new UserDto
                   {
                       Id = user.Id,
                       FullName = user.FullName,
                       Email = user.Email,
                       Role = (UserRoleEnum)user.RoleId,
                       CreatedAt = user.CreatedAt
                   }).ToListAsync();*/

            var Patients = await _context.Patients.Include(p => p.User).ToListAsync();

            return Patients.Select(patient => new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                User = new UserDto
                {
                    Id = patient.User.Id,
                    FullName = patient.User.FullName,
                    Email = patient.User.Email, 
                    Role = (UserRoleEnum)patient.User.RoleId,
                    CreatedAt = patient.User.CreatedAt,
                },

            }); // يتضمن الوصفات

        }
        public async Task<Patient> GetByIdAsync(int id)
        {
            /* var patient = await _context.Patients.Include(p => p.User).Include(p => p.Prescriptions).FirstOrDefaultAsync(p => p.Id == id);
             if (patient != null)
             {
                 return new Patient
                 {
                     Id = patient.Id,
                     UserId = patient.UserId,
                     DateOfBirth = patient.DateOfBirth,
                     Gender = patient.Gender,
                     BloodType = patient.BloodType,
                     User = new User
                     {
                         Id = patient.User.Id,
                         FullName = patient.User.FullName,
                         Email = patient.User.Email,
                         RoleId = patient.User.RoleId,
                         CreatedAt = patient.User.CreatedAt,
                     },

                 };
             }

             else
             {
                 return null;
             }*/

            return await _context.Patients
    .Include(p => p.User)
    .Include(p => p.Prescriptions)
    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PatientDashboardDto> GetDashboardDataAsync(int patientId)
        {
            // جلب عدد الوصفات الكلي
            var totalPrescriptions = await _context.Prescriptions
                .CountAsync(p => p.PatientId == patientId);

            // جلب عدد الأدوية المصروفة
            var dispensedMeds = await _context.Prescriptions
                .Where(p => p.PatientId == patientId && p.IsDispensed)
                .SelectMany(p => p.PrescriptionItems)
                .CountAsync();

            // جلب أحدث وصفة
            var latestPrescription = await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Include(p => p.Patient)
                .Include(p => p.Patient.User)
                .Include(p => p.Doctor)
                .ThenInclude(d => d.User)
                .Include(p => p.Doctor.MedicalCenter)
                .Include(p => p.PrescriptionItems)
                .OrderByDescending(p => p.IssuedDate)
                .FirstOrDefaultAsync();



            // تحويل البيانات إلى DTO
            var result = new PatientDashboardDto
            {
                TotalPrescriptions = totalPrescriptions,
                DispensedMeds = dispensedMeds,
                LatestPrescription = latestPrescription != null ? MapToPrescriptionDto(latestPrescription) : null
            };


         return result;
        }


        private PrescriptionDto MapToPrescriptionDto(Prescription prescription)
        {
            return new PrescriptionDto
            {
                Id = prescription.Id,
                DoctorId = prescription.DoctorId,
                PatientId = prescription.PatientId,
                IssuedDate = prescription.IssuedDate,
                IsDispensed = prescription.IsDispensed,
                Doctor = new DoctorDto
                {
                    Id = prescription.Doctor.Id,
                    UserId = prescription.Doctor.UserId,
                    MedicalCenterId = prescription.Doctor.MedicalCenterId,
                    Specialization = prescription.Doctor.Specialization,
                    LicenseNumber = prescription.Doctor.LicenseNumber,
                    User = new UserDto
                    {
                        Id = prescription.Doctor.User.Id,
                        FullName = prescription.Doctor.User.FullName,
                        Email = prescription.Doctor.User.Email,
                        Role = (UserRoleEnum)prescription.Doctor.User.RoleId,
                        CreatedAt = prescription.Doctor.User.CreatedAt,
                    },

                },
                Patient = new PatientDto
                {
                    Id = prescription.Patient.Id,
                    UserId = prescription.Patient.UserId,
                    Gender = prescription.Patient.Gender,
                    BloodType = prescription.Patient.BloodType,
                    DateOfBirth = prescription.Patient.DateOfBirth,
                    User = new UserDto
                    {
                        Id = prescription.Patient.User.Id,
                        FullName = prescription.Patient.User.FullName,
                        Email = prescription.Patient.User.Email,
                        Role = (UserRoleEnum)prescription.Patient.User.RoleId,
                        CreatedAt = prescription.Patient.User.CreatedAt,
                    },
                },
                PrescriptionItems = prescription.PrescriptionItems.Select(pi => new PrescriptionItemDto
                {
                    Id = pi.Id,
                    PrescriptionId = pi.PrescriptionId,
                    MedicationId = pi.MedicationId,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                }).ToList(),
            };

        }

        public async Task<Patient> GetPatientByUserIdAsync(int userId)
        {
            return await _context.Patients
        .Include(p => p.User)
        .Include(p => p.Prescriptions)
        .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<Patient>> SearchPatients(string term)
        {

            return _context.Patients
        .Include(p => p.User)
        .Include(p => p.Prescriptions)
        .Where(p =>
            p.User.FullName.Contains(term) ||
            p.User.Email.Contains(term) ||
            p.Id.ToString().Contains(term))
        .ToList();
             
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {


            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
    }
}
