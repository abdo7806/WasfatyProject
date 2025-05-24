using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;


public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<PrescriptionDto> GetByIdAsync(int id)
    {

        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription == null) return null;

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

                MedicalCenter = new MedicalCenterDto
                {
                    Id = prescription.Doctor.MedicalCenter.Id,
                    Name = prescription.Doctor.MedicalCenter.Name,
                    Address = prescription.Doctor.MedicalCenter.Address,
                    Phone = prescription.Doctor.MedicalCenter.Phone,
                }

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

    public async Task<IEnumerable<PrescriptionDto>> GetAllAsync()
    {
        var prescriptions = await _prescriptionRepository.GetAllAsync();
        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            DoctorId = p.DoctorId,
            PatientId = p.PatientId,
            IssuedDate = p.IssuedDate,
            IsDispensed = p.IsDispensed,
            Doctor = new DoctorDto
            {
                Id = p.Doctor.Id,
                UserId = p.Doctor.UserId,
                MedicalCenterId = p.Doctor.MedicalCenterId,
                Specialization = p.Doctor.Specialization,
                LicenseNumber = p.Doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = p.Doctor.User.Id,
                    FullName = p.Doctor.User.FullName,
                    Email = p.Doctor.User.Email,
                    Role = (UserRoleEnum)p.Doctor.User.RoleId,
                    CreatedAt = p.Doctor.User.CreatedAt,
                },
                MedicalCenter = new MedicalCenterDto
                {
                    Id = p.Doctor.MedicalCenter.Id,
                    Name = p.Doctor.MedicalCenter.Name,
                    Address = p.Doctor.MedicalCenter.Address,
                    Phone = p.Doctor.MedicalCenter.Phone,
                }

            },
            Patient = new PatientDto
            {
                Id = p.Patient.Id,
                UserId = p.Patient.UserId,
                Gender = p.Patient.Gender,
                BloodType = p.Patient.BloodType,
                DateOfBirth = p.Patient.DateOfBirth,
                User = new UserDto
                {
                    Id = p.Patient.User.Id,
                    FullName = p.Patient.User.FullName,
                    Email = p.Patient.User.Email,
                    Role = (UserRoleEnum)p.Patient.User.RoleId,
                    CreatedAt = p.Patient.User.CreatedAt,
                },
            },
                PrescriptionItems = p.PrescriptionItems.Select(pi => new PrescriptionItemDto
                {
                    Id = pi.Id,
                    PrescriptionId = pi.PrescriptionId,
                    MedicationId = pi.MedicationId,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                    
                }).ToList(),
        }).ToList();
   
    }

    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto prescriptionDto)
    {
        var prescription = new Prescription
        {
            DoctorId = prescriptionDto.DoctorId,
            PatientId = prescriptionDto.PatientId,
            IssuedDate = prescriptionDto.IssuedDate,
            IsDispensed = prescriptionDto.IsDispensed,
   
        };

        var addedPrescription = await _prescriptionRepository.AddAsync(prescription);
        return new PrescriptionDto
        {
            Id = addedPrescription.Id,
            DoctorId = addedPrescription.DoctorId,
            PatientId = addedPrescription.PatientId,
            IssuedDate = addedPrescription.IssuedDate,
            IsDispensed = addedPrescription.IsDispensed,
        };
    }

    public async Task<PrescriptionDto> UpdateAsync(int id, CreatePrescriptionDto prescriptionDto)
    {
        var existingPrescription = await _prescriptionRepository.GetByIdAsync(id);
        if (existingPrescription == null) return null;

        existingPrescription.DoctorId = prescriptionDto.DoctorId;
        existingPrescription.PatientId = prescriptionDto.PatientId;
        existingPrescription.IssuedDate = prescriptionDto.IssuedDate;
        existingPrescription.IsDispensed = prescriptionDto.IsDispensed;

        Prescription prescription = await _prescriptionRepository.UpdateAsync(existingPrescription);
        return new PrescriptionDto
        {
            Id = prescription.Id,
            DoctorId = prescription.DoctorId,
            PatientId = prescription.PatientId,
            IssuedDate = prescription.IssuedDate,
            IsDispensed = prescription.IsDispensed,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
       return await _prescriptionRepository.DeleteAsync(id);
    }

    public async Task<List<PrescriptionDto>> GetByDoctorIdAsync(int doctorId)
    {
        var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctorId);

        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            DoctorId = p.DoctorId,
            PatientId = p.PatientId,
            IssuedDate = p.IssuedDate,
            IsDispensed = p.IsDispensed,
            Doctor = new DoctorDto
            {
                Id = p.Doctor.Id,
                UserId = p.Doctor.UserId,
                MedicalCenterId = p.Doctor.MedicalCenterId,
                Specialization = p.Doctor.Specialization,
                LicenseNumber = p.Doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = p.Doctor.User.Id,
                    FullName = p.Doctor.User.FullName,
                    Email = p.Doctor.User.Email,
                    Role = (UserRoleEnum)p.Doctor.User.RoleId,
                    CreatedAt = p.Doctor.User.CreatedAt,
                },

            },
            Patient = new PatientDto
            {
                Id = p.Patient.Id,
                UserId = p.Patient.UserId,
                Gender = p.Patient.Gender,
                BloodType = p.Patient.BloodType,
                DateOfBirth = p.Patient.DateOfBirth,
                User = new UserDto
                {
                    Id = p.Patient.User.Id,
                    FullName = p.Patient.User.FullName,
                    Email = p.Patient.User.Email,
                    Role = (UserRoleEnum)p.Patient.User.RoleId,
                    CreatedAt = p.Patient.User.CreatedAt,
                },
            },
            PrescriptionItems = p.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();
    }

    public async Task<List<PrescriptionDto>> GetByPatientIdAsync(int PatientId)
    {
        var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(PatientId);
        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            DoctorId = p.DoctorId,
            PatientId = p.PatientId,
            IssuedDate = p.IssuedDate,
            IsDispensed = p.IsDispensed,
            Doctor = new DoctorDto
            {
                Id = p.Doctor.Id,
                UserId = p.Doctor.UserId,
                MedicalCenterId = p.Doctor.MedicalCenterId,
                Specialization = p.Doctor.Specialization,
                LicenseNumber = p.Doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = p.Doctor.User.Id,
                    FullName = p.Doctor.User.FullName,
                    Email = p.Doctor.User.Email,
                    Role = (UserRoleEnum)p.Doctor.User.RoleId,
                    CreatedAt = p.Doctor.User.CreatedAt,
                },

                MedicalCenter = new MedicalCenterDto
                {
                    Id = p.Doctor.MedicalCenter.Id,
                    Name = p.Doctor.MedicalCenter.Name,
                    Address = p.Doctor.MedicalCenter.Address,
                    Phone = p.Doctor.MedicalCenter.Phone,
                }

            },
            Patient = new PatientDto
            {
                Id = p.Patient.Id,
                UserId = p.Patient.UserId,
                Gender = p.Patient.Gender,
                BloodType = p.Patient.BloodType,
                DateOfBirth = p.Patient.DateOfBirth,
                User = new UserDto
                {
                    Id = p.Patient.User.Id,
                    FullName = p.Patient.User.FullName,
                    Email = p.Patient.User.Email,
                    Role = (UserRoleEnum)p.Patient.User.RoleId,
                    CreatedAt = p.Patient.User.CreatedAt,
                },
            },
            PrescriptionItems = p.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();
    }

    public async Task<PrescriptiontDashboardDto> GetDashboardDataAsync()
    {
        return await _prescriptionRepository.GetDashboardDataAsync();
    }

    public async Task<List<PrescriptionDto>> GetAllPrescriptionPendingAsync()
    {
        var prescriptions = await _prescriptionRepository.GetAllPrescriptionPendingAsync();
        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            DoctorId = p.DoctorId,
            PatientId = p.PatientId,
            IssuedDate = p.IssuedDate,
            IsDispensed = p.IsDispensed,
            Doctor = new DoctorDto
            {
                Id = p.Doctor.Id,
                UserId = p.Doctor.UserId,
                MedicalCenterId = p.Doctor.MedicalCenterId,
                Specialization = p.Doctor.Specialization,
                LicenseNumber = p.Doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = p.Doctor.User.Id,
                    FullName = p.Doctor.User.FullName,
                    Email = p.Doctor.User.Email,
                    Role = (UserRoleEnum)p.Doctor.User.RoleId,
                    CreatedAt = p.Doctor.User.CreatedAt,
                },
                MedicalCenter = new MedicalCenterDto
                {
                    Id = p.Doctor.MedicalCenter.Id,
                    Name = p.Doctor.MedicalCenter.Name,
                    Address = p.Doctor.MedicalCenter.Address,
                    Phone = p.Doctor.MedicalCenter.Phone,
                }

            },
            Patient = new PatientDto
            {
                Id = p.Patient.Id,
                UserId = p.Patient.UserId,
                Gender = p.Patient.Gender,
                BloodType = p.Patient.BloodType,
                DateOfBirth = p.Patient.DateOfBirth,
                User = new UserDto
                {
                    Id = p.Patient.User.Id,
                    FullName = p.Patient.User.FullName,
                    Email = p.Patient.User.Email,
                    Role = (UserRoleEnum)p.Patient.User.RoleId,
                    CreatedAt = p.Patient.User.CreatedAt,
                },
            },
            PrescriptionItems = p.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();
    }

    public async Task<List<PrescriptionDto>> GetNewPrescriptionsAsync(int lastPrescriptionId)
    {
        var prescriptions = await _prescriptionRepository.GetNewPrescriptionsAsync(lastPrescriptionId);
        return prescriptions.Select(p => new PrescriptionDto
        {
            Id = p.Id,
            DoctorId = p.DoctorId,
            PatientId = p.PatientId,
            IssuedDate = p.IssuedDate,
            IsDispensed = p.IsDispensed,
            Doctor = new DoctorDto
            {
                Id = p.Doctor.Id,
                UserId = p.Doctor.UserId,
                MedicalCenterId = p.Doctor.MedicalCenterId,
                Specialization = p.Doctor.Specialization,
                LicenseNumber = p.Doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = p.Doctor.User.Id,
                    FullName = p.Doctor.User.FullName,
                    Email = p.Doctor.User.Email,
                    Role = (UserRoleEnum)p.Doctor.User.RoleId,
                    CreatedAt = p.Doctor.User.CreatedAt,
                },
                MedicalCenter = new MedicalCenterDto
                {
                    Id = p.Doctor.MedicalCenter.Id,
                    Name = p.Doctor.MedicalCenter.Name,
                    Address = p.Doctor.MedicalCenter.Address,
                    Phone = p.Doctor.MedicalCenter.Phone,
                }

            },
            Patient = new PatientDto
            {
                Id = p.Patient.Id,
                UserId = p.Patient.UserId,
                Gender = p.Patient.Gender,
                BloodType = p.Patient.BloodType,
                DateOfBirth = p.Patient.DateOfBirth,
                User = new UserDto
                {
                    Id = p.Patient.User.Id,
                    FullName = p.Patient.User.FullName,
                    Email = p.Patient.User.Email,
                    Role = (UserRoleEnum)p.Patient.User.RoleId,
                    CreatedAt = p.Patient.User.CreatedAt,
                },
            },
            PrescriptionItems = p.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();
    }
}