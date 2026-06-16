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
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPrescriptionItemRepository _prescriptionItemRepository;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    public PrescriptionService(IPrescriptionRepository prescriptionRepository, 
        IPrescriptionItemRepository prescriptionItemRepository, 
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _prescriptionRepository = prescriptionRepository;
        _prescriptionItemRepository = prescriptionItemRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();

    }
    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto prescriptionDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var prescription = new Prescription
            {
                DoctorId = prescriptionDto.DoctorId,
                PatientId = prescriptionDto.PatientId,
                IssuedDate = prescriptionDto.IssuedDate,
                IsDispensed = prescriptionDto.IsDispensed,
            };

            var addedPrescription =
                await _prescriptionRepository.AddAsync(prescription);

            var addedItems = new List<PrescriptionItem>();

            foreach (var prescriptionItemDto in prescriptionDto.PrescriptionItems)
            {
                // التحقق من صحة البيانات
                if (prescriptionItemDto.MedicationId == null &&
                    (string.IsNullOrWhiteSpace(prescriptionItemDto.CustomMedicationName) ||
                     string.IsNullOrWhiteSpace(prescriptionItemDto.CustomMedicationDescription)))
                {
                    await _unitOfWork.RollbackAsync();

                    // ← تسجيل فشل إنشاء الوصفة
                    await _auditService.LogAsync(
                        action: "CreatePrescriptionFailed",
                        entityName: "Prescription",
                        details: $"Failed to create prescription - Invalid medication data for PatientId: {prescriptionDto.PatientId}");

                    return null;
                }

                var prescriptionItem = new PrescriptionItem
                {
                    PrescriptionId = addedPrescription.Id,
                    MedicationId = prescriptionItemDto.MedicationId,
                    CustomMedicationName = prescriptionItemDto.CustomMedicationName,
                    CustomMedicationDescription = prescriptionItemDto.CustomMedicationDescription,
                    CustomDosageForm = prescriptionItemDto.CustomDosageForm,
                    CustomStrength = prescriptionItemDto.CustomStrength,
                    Dosage = prescriptionItemDto.Dosage,
                    Frequency = prescriptionItemDto.Frequency,
                    Duration = prescriptionItemDto.Duration
                };

                var addedPrescriptionItem =
                    await _prescriptionItemRepository.AddAsync(prescriptionItem);

                addedItems.Add(addedPrescriptionItem);
            }

            await _unitOfWork.CommitAsync();

            // ← تسجيل نجاح إنشاء الوصفة
            await _auditService.LogAsync(
                action: "CreatePrescription",
                entityName: "Prescription",
                entityId: addedPrescription.Id.ToString(),
                details: $"Prescription created for PatientId: {prescriptionDto.PatientId} with {prescriptionDto.PrescriptionItems?.Count ?? 0} items");


            return new PrescriptionDto
            {
                Id = addedPrescription.Id,
                DoctorId = addedPrescription.DoctorId,
                PatientId = addedPrescription.PatientId,
                IssuedDate = addedPrescription.IssuedDate,
                IsDispensed = addedPrescription.IsDispensed,

                PrescriptionItems = addedItems.Select(pi => new PrescriptionItemDto
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
                    Duration = pi.Duration
                }).ToList()
            };
        }
        catch(Exception ex )
        {
            await _unitOfWork.RollbackAsync();

            // ← تسجيل خطأ في الإنشاء
            await _auditService.LogAsync(
                action: "CreatePrescriptionError",
                entityName: "Prescription",
                details: $"Error creating prescription for PatientId: {prescriptionDto.PatientId} - Error: {ex.Message}");

            return null;
        }
    }
    public async Task<PrescriptionDto> UpdateAsync
    (
        int id,
        CreatePrescriptionDto prescriptionDto
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingPrescription =
                await _prescriptionRepository.GetByIdAsync(id);

            if (existingPrescription == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (الوصفة غير موجودة)
                await _auditService.LogAsync(
                    action: "UpdatePrescriptionFailed",
                    entityName: "Prescription",
                    entityId: id.ToString(),
                    details: $"Failed to update prescription - Prescription with ID {id} not found");

                return null;
            }

            // حفظ القيم القديمة للتسجيل
            var oldDoctorId = existingPrescription.DoctorId;
            var oldPatientId = existingPrescription.PatientId;
            var oldIsDispensed = existingPrescription.IsDispensed;


            // تحديث الرأس
            existingPrescription.DoctorId = prescriptionDto.DoctorId;
            existingPrescription.PatientId = prescriptionDto.PatientId;
            existingPrescription.IssuedDate = prescriptionDto.IssuedDate;
            existingPrescription.IsDispensed = prescriptionDto.IsDispensed;

            var updatedPrescription =
                await _prescriptionRepository.UpdateAsync(existingPrescription);

            // حذف التفاصيل القديمة
            await _prescriptionItemRepository
                .DeleteByPrescriptionIdAsync(updatedPrescription.Id);

            var addedItems = new List<PrescriptionItem>();

            // إضافة التفاصيل الجديدة
            foreach (var prescriptionItemDto in prescriptionDto.PrescriptionItems)
            {
                // التحقق من صحة البيانات
                if (prescriptionItemDto.MedicationId == null &&
                    (string.IsNullOrWhiteSpace(prescriptionItemDto.CustomMedicationName) ||
                     string.IsNullOrWhiteSpace(prescriptionItemDto.CustomMedicationDescription)))
                {
                    await _unitOfWork.RollbackAsync();

                    // ← تسجيل فشل التحديث (بيانات غير صالحة)
                    await _auditService.LogAsync(
                        action: "UpdatePrescriptionFailed",
                        entityName: "Prescription",
                        entityId: id.ToString(),
                        details: $"Failed to update prescription - Invalid medication data");

                    return null;
                }

                // ← تسجيل نجاح التحديث
                var changes = new List<string>();
                if (oldDoctorId != prescriptionDto.DoctorId) changes.Add($"DoctorId: {oldDoctorId} -> {prescriptionDto.DoctorId}");
                if (oldPatientId != prescriptionDto.PatientId) changes.Add($"PatientId: {oldPatientId} -> {prescriptionDto.PatientId}");
                if (oldIsDispensed != prescriptionDto.IsDispensed) changes.Add($"IsDispensed: {oldIsDispensed} -> {prescriptionDto.IsDispensed}");

                await _auditService.LogAsync(
                    action: "UpdatePrescription",
                    entityName: "Prescription",
                    entityId: id.ToString(),
                    details: $"Prescription updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes to main fields, items updated")}");


                var prescriptionItem = new PrescriptionItem
                {
                    PrescriptionId = updatedPrescription.Id,
                    MedicationId = prescriptionItemDto.MedicationId,
                    CustomMedicationName = prescriptionItemDto.CustomMedicationName,
                    CustomMedicationDescription = prescriptionItemDto.CustomMedicationDescription,
                    CustomDosageForm = prescriptionItemDto.CustomDosageForm,
                    CustomStrength = prescriptionItemDto.CustomStrength,
                    Dosage = prescriptionItemDto.Dosage,
                    Frequency = prescriptionItemDto.Frequency,
                    Duration = prescriptionItemDto.Duration
                };

                var addedPrescriptionItem =
                    await _prescriptionItemRepository.AddAsync(prescriptionItem);

                addedItems.Add(addedPrescriptionItem);
            }

            await _unitOfWork.CommitAsync();

            return new PrescriptionDto
            {
                Id = updatedPrescription.Id,
                DoctorId = updatedPrescription.DoctorId,
                PatientId = updatedPrescription.PatientId,
                IssuedDate = updatedPrescription.IssuedDate,
                IsDispensed = updatedPrescription.IsDispensed,

                PrescriptionItems = addedItems.Select(pi => new PrescriptionItemDto
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
                    Duration = pi.Duration
                }).ToList()
            };
        }
        catch(Exception ex) 
        {
            await _unitOfWork.RollbackAsync();


            // ← تسجيل خطأ في التحديث
            await _auditService.LogAsync(
                action: "UpdatePrescriptionError",
                entityName: "Prescription",
                entityId: id.ToString(),
                details: $"Error updating prescription ID {id} - Error: {ex.Message}");

            return null;
        }
    }
   
    public async Task<bool> DeleteAsync(int id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);

        if (prescription == null)
        {
            // ← تسجيل فشل الحذف (الوصفة غير موجودة)
            await _auditService.LogAsync(
                action: "DeletePrescriptionFailed",
                entityName: "Prescription",
                entityId: id.ToString(),
                details: $"Failed to delete prescription - Prescription with ID {id} not found");

            return false;
        }

        var result = await _prescriptionRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeletePrescription",
                entityName: "Prescription",
                entityId: id.ToString(),
                details: $"Prescription deleted successfully for PatientId: {prescription.PatientId}");
        }

        return result;
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
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
                CustomMedicationName = pi.CustomMedicationName,
                CustomMedicationDescription = pi.CustomMedicationDescription,
                CustomDosageForm = pi.CustomDosageForm,
                CustomStrength = pi.CustomStrength,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,

            }).ToList(),
        }).ToList();
    }
}