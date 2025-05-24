using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

public class DispenseRecordService : IDispenseRecordService
{
    private readonly IDispenseRecordRepository _dispenseRecordRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;

    public DispenseRecordService(IDispenseRecordRepository dispenseRecordRepository, IPrescriptionRepository prescriptionRepository)
    {
        _dispenseRecordRepository = dispenseRecordRepository;
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<DispenseRecordDto?> GetByIdAsync(int id)
    {
        var dispenseRecord = await _dispenseRecordRepository.GetByIdAsync(id);
        if (dispenseRecord == null) return null;

        return new DispenseRecordDto
        {
            Id = dispenseRecord.Id,
            PrescriptionId = dispenseRecord.PrescriptionId,
            PharmacistId = dispenseRecord.PharmacistId,
            PharmacyId = dispenseRecord.PharmacyId,
            DispensedDate = dispenseRecord.DispensedDate,
            //  PharmacistName = dispenseRecord.Pharmacist.User.FullName,
            //  PharmacyName = dispenseRecord.Pharmacy.Name,

            Pharmacist = new PharmacistDto
            {
                Id = dispenseRecord.Pharmacist.Id,
                UserId = dispenseRecord.Pharmacist.UserId,
                PharmacyId = dispenseRecord.Pharmacist.PharmacyId,
                LicenseNumber = dispenseRecord.Pharmacist.LicenseNumber,
                DispenseRecords = dispenseRecord.Pharmacist.DispenseRecords.Select(dispenseRecord2 => new DispenseRecordDto
                {
                    Id = dispenseRecord2.Id,
                    PrescriptionId = dispenseRecord2.PrescriptionId,
                    PharmacistId = dispenseRecord2.PharmacistId,
                    PharmacyId = dispenseRecord2.PharmacyId,
                    DispensedDate = dispenseRecord2.DispensedDate
                }).ToList(),

                User = new UserDto
                {
                    Id = dispenseRecord.Pharmacist.User.Id,
                    FullName = dispenseRecord.Pharmacist.User.FullName,
                    Email = dispenseRecord.Pharmacist.User.Email,
                    Role = (UserRoleEnum)dispenseRecord.Pharmacist.User.RoleId,
                    CreatedAt = dispenseRecord.Pharmacist.User.CreatedAt,


                },
            },
            Pharmacy = new PharmacyDto
            {
                Id = dispenseRecord.Pharmacy.Id,
                Name = dispenseRecord.Pharmacy.Name,
                Address = dispenseRecord.Pharmacy.Address,
                Phone = dispenseRecord.Pharmacy.Phone,
            },
            Prescription = new PrescriptionDto
            {
                Id = dispenseRecord.Prescription.Id,
                DoctorId = dispenseRecord.Prescription.DoctorId,
                PatientId = dispenseRecord.Prescription.PatientId,
                IssuedDate = dispenseRecord.Prescription.IssuedDate,
                IsDispensed = dispenseRecord.Prescription.IsDispensed,
                Doctor = new DoctorDto
                {
                    Id = dispenseRecord.Prescription.Doctor.Id,
                    UserId = dispenseRecord.Prescription.Doctor.UserId,
                    MedicalCenterId = dispenseRecord.Prescription.Doctor.MedicalCenterId,
                    Specialization = dispenseRecord.Prescription.Doctor.Specialization,
                    LicenseNumber = dispenseRecord.Prescription.Doctor.LicenseNumber,
                    User = new UserDto
                    {
                        Id = dispenseRecord.Prescription.Doctor.User.Id,
                        FullName = dispenseRecord.Prescription.Doctor.User.FullName,
                        Email = dispenseRecord.Prescription.Doctor.User.Email,
                        Role = (UserRoleEnum)dispenseRecord.Prescription.Doctor.User.RoleId,
                        CreatedAt = dispenseRecord.Prescription.Doctor.User.CreatedAt,
                    },

                },
                Patient = new PatientDto
                {
                    Id = dispenseRecord.Prescription.Patient.Id,
                    UserId = dispenseRecord.Prescription.Patient.UserId,
                    Gender = dispenseRecord.Prescription.Patient.Gender,
                    BloodType = dispenseRecord.Prescription.Patient.BloodType,
                    DateOfBirth = dispenseRecord.Prescription.Patient.DateOfBirth,
                    User = new UserDto
                    {
                        Id = dispenseRecord.Prescription.Patient.User.Id,
                        FullName = dispenseRecord.Prescription.Patient.User.FullName,
                        Email = dispenseRecord.Prescription.Patient.User.Email,
                        Role = (UserRoleEnum)dispenseRecord.Prescription.Patient.User.RoleId,
                        CreatedAt = dispenseRecord.Prescription.Patient.User.CreatedAt,
                    },
                },
                PrescriptionItems = dispenseRecord.Prescription.PrescriptionItems.Select(pi => new PrescriptionItemDto
                {
                    Id = pi.Id,
                    PrescriptionId = pi.PrescriptionId,
                    MedicationId = pi.MedicationId,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                  //  MedicationName = pi.Medication.Name

                }).ToList(),
            }

        };
    }

    public async Task<List<DispenseRecordDto>> GetAllAsync()
    {
        var dispenseRecords = await _dispenseRecordRepository.GetAllAsync();
        return dispenseRecords.Select(dr => new DispenseRecordDto
        {
            Id = dr.Id,
            PrescriptionId = dr.PrescriptionId,
            PharmacistId = dr.PharmacistId,
            PharmacyId = dr.PharmacyId,
            DispensedDate = dr.DispensedDate,
            //  PharmacistName = dr.Pharmacist.User.FullName,
            //  PharmacyName = dr.Pharmacy.Name,

            Pharmacist = new PharmacistDto
            {
                Id = dr.Pharmacist.Id,
                UserId = dr.Pharmacist.UserId,
                PharmacyId = dr.Pharmacist.PharmacyId,
                LicenseNumber = dr.Pharmacist.LicenseNumber,
                DispenseRecords = dr.Pharmacist.DispenseRecords.Select(dr2 => new DispenseRecordDto
                {
                    Id = dr2.Id,
                    PrescriptionId = dr2.PrescriptionId,
                    PharmacistId = dr2.PharmacistId,
                    PharmacyId = dr2.PharmacyId,
                    DispensedDate = dr2.DispensedDate
                }).ToList(),

                User = new UserDto
                {
                    Id = dr.Pharmacist.User.Id,
                    FullName = dr.Pharmacist.User.FullName,
                    Email = dr.Pharmacist.User.Email,
                    Role = (UserRoleEnum)dr.Pharmacist.User.RoleId,
                    CreatedAt = dr.Pharmacist.User.CreatedAt,


                },
            },
            Pharmacy = new PharmacyDto
            {
                Id = dr.Pharmacy.Id,
                Name = dr.Pharmacy.Name,
                Address = dr.Pharmacy.Address,
                Phone = dr.Pharmacy.Phone,
            },
           Prescription = new PrescriptionDto
           {
               Id = dr.Prescription.Id,
               DoctorId = dr.Prescription.DoctorId,
               PatientId = dr.Prescription.PatientId,
               IssuedDate = dr.Prescription.IssuedDate,
               IsDispensed = dr.Prescription.IsDispensed,
               Doctor = new DoctorDto
               {
                   Id = dr.Prescription.Doctor.Id,
                   UserId = dr.Prescription.Doctor.UserId,
                   MedicalCenterId = dr.Prescription.Doctor.MedicalCenterId,
                   Specialization = dr.Prescription.Doctor.Specialization,
                   LicenseNumber = dr.Prescription.Doctor.LicenseNumber,
                   User = new UserDto
                   {
                       Id = dr.Prescription.Doctor.User.Id,
                       FullName = dr.Prescription.Doctor.User.FullName,
                       Email = dr.Prescription.Doctor.User.Email,
                       Role = (UserRoleEnum)dr.Prescription.Doctor.User.RoleId,
                       CreatedAt = dr.Prescription.Doctor.User.CreatedAt,
                   },

               },
               Patient = new PatientDto
               {
                   Id = dr.Prescription.Patient.Id,
                   UserId = dr.Prescription.Patient.UserId,
                   Gender = dr.Prescription.Patient.Gender,
                   BloodType = dr.Prescription.Patient.BloodType,
                   DateOfBirth = dr.Prescription.Patient.DateOfBirth,
                   User = new UserDto
                   {
                       Id = dr.Prescription.Patient.User.Id,
                       FullName = dr.Prescription.Patient.User.FullName,
                       Email = dr.Prescription.Patient.User.Email,
                       Role = (UserRoleEnum)dr.Prescription.Patient.User.RoleId,
                       CreatedAt = dr.Prescription.Patient.User.CreatedAt,
                   },
               },
               PrescriptionItems = dr.Prescription.PrescriptionItems.Select(pi => new PrescriptionItemDto
               {
                   Id = pi.Id,
                   PrescriptionId = pi.PrescriptionId,
                   MedicationId = pi.MedicationId,
                   Dosage = pi.Dosage,
                   Frequency = pi.Frequency,
                   Duration = pi.Duration,
              //     MedicationName = pi.Medication.Name

               }).ToList(),
           }
        }).ToList();
  
    }

    public async Task<DispenseRecordDto> CreateAsync(CreateDispenseRecordDto dispenseRecordDto)
    {

        var existingPrescription = await _prescriptionRepository.GetByIdAsync(dispenseRecordDto.PrescriptionId);
        if (existingPrescription == null) return null;



        var dispenseRecord = new DispenseRecord
        {
            PrescriptionId = dispenseRecordDto.PrescriptionId,
            PharmacistId = dispenseRecordDto.PharmacistId,
            PharmacyId = dispenseRecordDto.PharmacyId,
            DispensedDate = dispenseRecordDto.DispensedDate,
        };

        var addedDispenseRecord = await _dispenseRecordRepository.AddAsync(dispenseRecord, existingPrescription);
        if(addedDispenseRecord == null)
        {
            return null;
        }
        return new DispenseRecordDto
        {
            Id = addedDispenseRecord.Id,
            PrescriptionId = addedDispenseRecord.PrescriptionId,
            PharmacistId = addedDispenseRecord.PharmacistId,
            PharmacyId = addedDispenseRecord.PharmacyId,
            DispensedDate = addedDispenseRecord.DispensedDate,
        };
    }


    public async Task<DispenseRecordDto> UpdateAsync(int id, CreateDispenseRecordDto dispenseRecordDto)
    {
        var existingDispenseRecord = await _dispenseRecordRepository.GetByIdAsync(id);
        if (existingDispenseRecord == null) return null;

        existingDispenseRecord.PrescriptionId = dispenseRecordDto.PrescriptionId;
        existingDispenseRecord.PharmacistId = dispenseRecordDto.PharmacistId;
        existingDispenseRecord.PharmacyId = dispenseRecordDto.PharmacyId;
        existingDispenseRecord.DispensedDate = dispenseRecordDto.DispensedDate;

        DispenseRecord DispenseRecord = await _dispenseRecordRepository.UpdateAsync(existingDispenseRecord);
        return new DispenseRecordDto
        {
            Id = DispenseRecord.Id,
            PrescriptionId = DispenseRecord.PrescriptionId,
            PharmacistId = DispenseRecord.PharmacistId,
            PharmacyId = DispenseRecord.PharmacyId,
            DispensedDate = DispenseRecord.DispensedDate,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _dispenseRecordRepository.DeleteAsync(id);
    }

    public async Task<List<DispenseRecordDto>> GetByPharmacyIdAsync(int PharmacyId)
    {
        var dispenseRecords = await _dispenseRecordRepository.GetByPharmacyIdAsync(PharmacyId);
        return dispenseRecords.Select(dr => new DispenseRecordDto
        {
            Id = dr.Id,
            PrescriptionId = dr.PrescriptionId,
            PharmacistId = dr.PharmacistId,
            PharmacyId = dr.PharmacyId,
            DispensedDate = dr.DispensedDate,
            //  PharmacistName = dr.Pharmacist.User.FullName,
            //  PharmacyName = dr.Pharmacy.Name,

            Pharmacist = new PharmacistDto
            {
                Id = dr.Pharmacist.Id,
                UserId = dr.Pharmacist.UserId,
                PharmacyId = dr.Pharmacist.PharmacyId,
                LicenseNumber = dr.Pharmacist.LicenseNumber,
                DispenseRecords = dr.Pharmacist.DispenseRecords.Select(dr2 => new DispenseRecordDto
                {
                    Id = dr2.Id,
                    PrescriptionId = dr2.PrescriptionId,
                    PharmacistId = dr2.PharmacistId,
                    PharmacyId = dr2.PharmacyId,
                    DispensedDate = dr2.DispensedDate
                }).ToList(),

                User = new UserDto
                {
                    Id = dr.Pharmacist.User.Id,
                    FullName = dr.Pharmacist.User.FullName,
                    Email = dr.Pharmacist.User.Email,
                    Role = (UserRoleEnum)dr.Pharmacist.User.RoleId,
                    CreatedAt = dr.Pharmacist.User.CreatedAt,


                },
            },
            Pharmacy = new PharmacyDto
            {
                Id = dr.Pharmacy.Id,
                Name = dr.Pharmacy.Name,
                Address = dr.Pharmacy.Address,
                Phone = dr.Pharmacy.Phone,
            },
            Prescription = new PrescriptionDto
            {
                Id = dr.Prescription.Id,
                DoctorId = dr.Prescription.DoctorId,
                PatientId = dr.Prescription.PatientId,
                IssuedDate = dr.Prescription.IssuedDate,
                IsDispensed = dr.Prescription.IsDispensed,
                Doctor = new DoctorDto
                {
                    Id = dr.Prescription.Doctor.Id,
                    UserId = dr.Prescription.Doctor.UserId,
                    MedicalCenterId = dr.Prescription.Doctor.MedicalCenterId,
                    Specialization = dr.Prescription.Doctor.Specialization,
                    LicenseNumber = dr.Prescription.Doctor.LicenseNumber,
                    User = new UserDto
                    {
                        Id = dr.Prescription.Doctor.User.Id,
                        FullName = dr.Prescription.Doctor.User.FullName,
                        Email = dr.Prescription.Doctor.User.Email,
                        Role = (UserRoleEnum)dr.Prescription.Doctor.User.RoleId,
                        CreatedAt = dr.Prescription.Doctor.User.CreatedAt,
                    },

                },
                Patient = new PatientDto
                {
                    Id = dr.Prescription.Patient.Id,
                    UserId = dr.Prescription.Patient.UserId,
                    Gender = dr.Prescription.Patient.Gender,
                    BloodType = dr.Prescription.Patient.BloodType,
                    DateOfBirth = dr.Prescription.Patient.DateOfBirth,
                    User = new UserDto
                    {
                        Id = dr.Prescription.Patient.User.Id,
                        FullName = dr.Prescription.Patient.User.FullName,
                        Email = dr.Prescription.Patient.User.Email,
                        Role = (UserRoleEnum)dr.Prescription.Patient.User.RoleId,
                        CreatedAt = dr.Prescription.Patient.User.CreatedAt,
                    },
                },
                PrescriptionItems = dr.Prescription.PrescriptionItems.Select(pi => new PrescriptionItemDto
                {
                    Id = pi.Id,
                    PrescriptionId = pi.PrescriptionId,
                    MedicationId = pi.MedicationId,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                    //     MedicationName = pi.Medication.Name

                }).ToList(),


            }
        }).ToList();
    }
}
