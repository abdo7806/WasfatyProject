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
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;
namespace Wasfaty.Application.Services;

public class DispenseRecordService : IDispenseRecordService
{
    private readonly IDispenseRecordRepository _dispenseRecordRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IAuditService _auditService;

    public DispenseRecordService(IDispenseRecordRepository dispenseRecordRepository,
        IPrescriptionRepository prescriptionRepository,
        IAuditService auditService)
    {
        _dispenseRecordRepository = dispenseRecordRepository;
        _prescriptionRepository = prescriptionRepository;
        _auditService = auditService;
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
                    CustomMedicationName = pi.CustomMedicationName,
                    CustomMedicationDescription = pi.CustomMedicationDescription,
                    CustomDosageForm = pi.CustomDosageForm,
                    CustomStrength = pi.CustomStrength,
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
                   CustomMedicationName = pi.CustomMedicationName,
                   CustomMedicationDescription = pi.CustomMedicationDescription,
                   CustomDosageForm = pi.CustomDosageForm,
                   CustomStrength = pi.CustomStrength,
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

        if (existingPrescription == null)
        {
            // ← تسجيل فشل الإنشاء (الوصفة غير موجودة)
            await _auditService.LogAsync(
                action: "CreateDispenseRecordFailed",
                entityName: "DispenseRecord",
                details: $"Failed to create DispenseRecord - Prescription with ID {dispenseRecordDto.PrescriptionId} not found");

            return null;
        }

        // التحقق من أن الوصفة لم تصرف بعد
        if (existingPrescription.IsDispensed)
        {
            // ← تسجيل فشل الإنشاء (الوصفة مصروفة مسبقاً)
            await _auditService.LogAsync(
                action: "CreateDispenseRecordFailed",
                entityName: "DispenseRecord",
                details: $"Failed to create DispenseRecord - Prescription ID {dispenseRecordDto.PrescriptionId} is already dispensed");

            return null;
        }

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
            // ← تسجيل فشل الإنشاء (خطأ في الـ Repository)
            await _auditService.LogAsync(
                action: "CreateDispenseRecordFailed",
                entityName: "DispenseRecord",
                details: $"Failed to create DispenseRecord - Repository returned null for PrescriptionId: {dispenseRecordDto.PrescriptionId}");

            return null;
        }

        // ← تسجيل نجاح الإنشاء (صرف الدواء)
        await _auditService.LogAsync(
            action: "DispensePrescription",
            entityName: "Prescription",
            entityId: dispenseRecordDto.PrescriptionId.ToString(),
            details: $"Prescription dispensed successfully - PharmacistId: {dispenseRecordDto.PharmacistId}, PharmacyId: {dispenseRecordDto.PharmacyId}");

        // ← تسجيل إنشاء سجل الصرف
        await _auditService.LogAsync(
            action: "CreateDispenseRecord",
            entityName: "DispenseRecord",
            entityId: addedDispenseRecord.Id.ToString(),
            details: $"DispenseRecord created for PrescriptionId: {dispenseRecordDto.PrescriptionId}");

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
        
        if (existingDispenseRecord == null)
        {
            // ← تسجيل فشل التحديث (السجل غير موجود)
            await _auditService.LogAsync(
                action: "UpdateDispenseRecordFailed",
                entityName: "DispenseRecord",
                entityId: id.ToString(),
                details: $"Failed to update DispenseRecord - Record with ID {id} not found");

            return null;
        }

        // حفظ القيم القديمة للتسجيل
        var oldPrescriptionId = existingDispenseRecord.PrescriptionId;
        var oldPharmacistId = existingDispenseRecord.PharmacistId;
        var oldPharmacyId = existingDispenseRecord.PharmacyId;
        var oldDispensedDate = existingDispenseRecord.DispensedDate;

        existingDispenseRecord.PrescriptionId = dispenseRecordDto.PrescriptionId;
        existingDispenseRecord.PharmacistId = dispenseRecordDto.PharmacistId;
        existingDispenseRecord.PharmacyId = dispenseRecordDto.PharmacyId;
        existingDispenseRecord.DispensedDate = dispenseRecordDto.DispensedDate;

        DispenseRecord DispenseRecord = await _dispenseRecordRepository.UpdateAsync(existingDispenseRecord);

        var changes = new List<string>();
        if (oldPrescriptionId != dispenseRecordDto.PrescriptionId)
            changes.Add($"PrescriptionId: {oldPrescriptionId} -> {dispenseRecordDto.PrescriptionId}");
        if (oldPharmacistId != dispenseRecordDto.PharmacistId)
            changes.Add($"PharmacistId: {oldPharmacistId} -> {dispenseRecordDto.PharmacistId}");
        if (oldPharmacyId != dispenseRecordDto.PharmacyId)
            changes.Add($"PharmacyId: {oldPharmacyId} -> {dispenseRecordDto.PharmacyId}");
        if (oldDispensedDate != dispenseRecordDto.DispensedDate)
            changes.Add($"DispensedDate: {oldDispensedDate} -> {dispenseRecordDto.DispensedDate}");

        await _auditService.LogAsync(
            action: "UpdateDispenseRecord",
            entityName: "DispenseRecord",
            entityId: id.ToString(),
            details: $"DispenseRecord updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");


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
        var existingRecord = await _dispenseRecordRepository.GetByIdAsync(id);

        if (existingRecord == null)
        {
            // ← تسجيل فشل الحذف (السجل غير موجود)
            await _auditService.LogAsync(
                action: "DeleteDispenseRecordFailed",
                entityName: "DispenseRecord",
                entityId: id.ToString(),
                details: $"Failed to delete DispenseRecord - Record with ID {id} not found");

            return false;
        }

        // حفظ معلومات السجل قبل الحذف للتسجيل
        var prescriptionId = existingRecord.PrescriptionId;

        var result = await _dispenseRecordRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeleteDispenseRecord",
                entityName: "DispenseRecord",
                entityId: id.ToString(),
                details: $"DispenseRecord deleted successfully - Was associated with PrescriptionId: {prescriptionId}");
        }

        return result;
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
                    CustomMedicationName = pi.CustomMedicationName,
                    CustomMedicationDescription = pi.CustomMedicationDescription,
                    CustomDosageForm = pi.CustomDosageForm,
                    CustomStrength = pi.CustomStrength,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                    //     MedicationName = pi.Medication.Name

                }).ToList(),


            }
        }).ToList();
    }
}
