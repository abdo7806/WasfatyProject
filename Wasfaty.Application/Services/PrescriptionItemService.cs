using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Medications;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.Application.Services;

public class PrescriptionItemService : IPrescriptionItemService
{
    private readonly IPrescriptionItemRepository _prescriptionItemRepository;
    private readonly IMedicationRepository _medicationRepository;
    private readonly IAuditService _auditService;
    public PrescriptionItemService(IPrescriptionItemRepository prescriptionItemRepository, IMedicationRepository medicationRepository, IAuditService auditService)
    {
        _prescriptionItemRepository = prescriptionItemRepository;
        _medicationRepository = medicationRepository;
        _auditService = auditService;
    }


    public async Task<PrescriptionItemDto> GetByIdAsync(int id)
    {
        var item = await _prescriptionItemRepository.GetByIdAsync(id);
        if (item == null) return null;

        return new PrescriptionItemDto
        {
            Id = item.Id,
            PrescriptionId = item.PrescriptionId,
            MedicationId = item.MedicationId,
            CustomMedicationName = item.CustomMedicationName,
            CustomMedicationDescription = item.CustomMedicationDescription,
            CustomDosageForm = item.CustomDosageForm,
            CustomStrength = item.CustomStrength,
            Dosage = item.Dosage,
            Frequency = item.Frequency,
            Duration = item.Duration,
            Medication = item.Medication != null ? new MedicationDto
            {
                Id = item.Medication.Id,
                Name = item.Medication.Name,
                Description = item.Medication.Description,
                DosageForm = item.Medication.DosageForm,
                Strength = item.Medication.Strength
            } : null,
            Prescription = item.Prescription != null ? new PrescriptionDto
            {
                Id = item.Prescription.Id,
                DoctorId = item.Prescription.DoctorId,
                PatientId = item.Prescription.PatientId,
                IssuedDate = item.Prescription.IssuedDate,
                IsDispensed = item.Prescription.IsDispensed
            } : null
        };
    }


    public async Task<List<PrescriptionItemDto>> GetAllAsync()
    {
        var prescriptionItems = await _prescriptionItemRepository.GetAllAsync();
        return prescriptionItems.Select(item => new PrescriptionItemDto
        {
            Id = item.Id,
            PrescriptionId = item.PrescriptionId,
            MedicationId = item.MedicationId,
            CustomMedicationName = item.CustomMedicationName,
            CustomMedicationDescription = item.CustomMedicationDescription,
            CustomDosageForm = item.CustomDosageForm,
            CustomStrength = item.CustomStrength,
            Dosage = item.Dosage,
            Frequency = item.Frequency,
            Duration = item.Duration,
            Medication = item.Medication != null ? new MedicationDto
            {
                Id = item.Medication.Id,
                Name = item.Medication.Name,
                Description = item.Medication.Description,
                DosageForm = item.Medication.DosageForm,
                Strength = item.Medication.Strength
            } : null,
            Prescription = item.Prescription != null ? new PrescriptionDto
            {
                Id = item.Prescription.Id,
                DoctorId = item.Prescription.DoctorId,
                PatientId = item.Prescription.PatientId,
                IssuedDate = item.Prescription.IssuedDate,
                IsDispensed = item.Prescription.IsDispensed
            } : null
        }).ToList();

    }

    public async Task<PrescriptionItemDto> CreateAsync(CreatePrescriptionItemDto prescriptionItemDto)
    {
        // التحقق من صحة البيانات
        if (prescriptionItemDto.MedicationId == null &&
            (string.IsNullOrEmpty(prescriptionItemDto.CustomMedicationName) ||
            string.IsNullOrEmpty(prescriptionItemDto.CustomMedicationDescription)))
        {
            // ← تسجيل فشل الإنشاء (بيانات غير صالحة)
            await _auditService.LogAsync(
                action: "CreatePrescriptionItemFailed",
                entityName: "PrescriptionItem",
                details: $"Failed to create PrescriptionItem - Invalid data: No MedicationId and missing custom medication info");

            return null;
        }

        // إذا كان دواءً موجوداً، التحقق من وجوده
        if (prescriptionItemDto.MedicationId.HasValue)
        {
            var medicationExists = await _medicationRepository.GetAllAsync();
            if (medicationExists == null)
            {
                // ← تسجيل فشل الإنشاء (دواء غير موجود)
                await _auditService.LogAsync(
                    action: "CreatePrescriptionItemFailed",
                    entityName: "PrescriptionItem",
                    details: $"Failed to create PrescriptionItem - Medication with ID {prescriptionItemDto.MedicationId} not found");


                return null;

            }
        }

        var prescriptionItem = new PrescriptionItem
        {
            //PrescriptionId = prescriptionItemDto.PrescriptionId,
            MedicationId = prescriptionItemDto.MedicationId,
            CustomMedicationName = prescriptionItemDto.CustomMedicationName,
            CustomMedicationDescription = prescriptionItemDto.CustomMedicationDescription,
            CustomDosageForm = prescriptionItemDto.CustomDosageForm,
            CustomStrength = prescriptionItemDto.CustomStrength,
            Dosage = prescriptionItemDto.Dosage,
            Frequency = prescriptionItemDto.Frequency,
            Duration = prescriptionItemDto.Duration
        };

        var addedPrescriptionItem = await _prescriptionItemRepository.AddAsync(prescriptionItem);

        // ← تسجيل نجاح الإنشاء
        await _auditService.LogAsync(
            action: "CreatePrescriptionItem",
            entityName: "PrescriptionItem",
            entityId: addedPrescriptionItem.Id.ToString(),
            details: $"PrescriptionItem created for PrescriptionId: {addedPrescriptionItem.PrescriptionId} - " +
                     $"MedicationId: {prescriptionItemDto.MedicationId ?? 0}, " +
                     $"CustomMedication: {prescriptionItemDto.CustomMedicationName ?? "None"}");

        return new PrescriptionItemDto
        {
            Id = addedPrescriptionItem.Id,
            PrescriptionId = addedPrescriptionItem.PrescriptionId,
            MedicationId = addedPrescriptionItem.MedicationId,
            Dosage = addedPrescriptionItem.Dosage,
            Frequency = addedPrescriptionItem.Frequency,
            Duration = addedPrescriptionItem.Duration,


        };

    }

    public async Task<PrescriptionItemDto> UpdateAsync(int id, UpdatePrescriptionItemDto prescriptionItemDto)
    {
        var existingPrescriptionItem = await _prescriptionItemRepository.GetByIdAsync(id);
        if (existingPrescriptionItem == null) return null;

        // التحقق من صحة البيانات
        if (prescriptionItemDto.MedicationId == null &&
            (string.IsNullOrEmpty(prescriptionItemDto.CustomMedicationName) ||
            string.IsNullOrEmpty(prescriptionItemDto.CustomMedicationDescription)))
        {
            // ← تسجيل فشل التحديث (العنصر غير موجود)
            await _auditService.LogAsync(
                action: "UpdatePrescriptionItemFailed",
                entityName: "PrescriptionItem",
                entityId: id.ToString(),
                details: $"Failed to update PrescriptionItem - Item with ID {id} not found");
        
            return null;
        }

        // إذا كان دواءً موجوداً، التحقق من وجوده
        if (prescriptionItemDto.MedicationId.HasValue)
        {
            var medicationExists = await _medicationRepository.GetByIdAsync(prescriptionItemDto.MedicationId ?? 0);
            if (medicationExists == null)
            {
                await _auditService.LogAsync(
                         action: "UpdatePrescriptionItemFailed",
                         entityName: "PrescriptionItem",
                         entityId: id.ToString(),
                         details: $"Failed to update PrescriptionItem ID {id} - Medication with ID {prescriptionItemDto.MedicationId} not found");
                throw new KeyNotFoundException("الدواء المحدد غير موجود");
            }
        }

        // حفظ القيم القديمة للتسجيل
        var oldMedicationId = existingPrescriptionItem.MedicationId;
        var oldCustomName = existingPrescriptionItem.CustomMedicationName;
        var oldDosage = existingPrescriptionItem.Dosage;


        existingPrescriptionItem.MedicationId = prescriptionItemDto.MedicationId;
        existingPrescriptionItem.CustomMedicationName = prescriptionItemDto.CustomMedicationName;
        existingPrescriptionItem.CustomMedicationDescription = prescriptionItemDto.CustomMedicationDescription;
        existingPrescriptionItem.CustomDosageForm = prescriptionItemDto.CustomDosageForm;
        existingPrescriptionItem.CustomStrength = prescriptionItemDto.CustomStrength;
        existingPrescriptionItem.Dosage = prescriptionItemDto.Dosage;
        existingPrescriptionItem.Frequency = prescriptionItemDto.Frequency;
        existingPrescriptionItem.Duration = prescriptionItemDto.Duration;

        PrescriptionItem PrescriptionItem = await _prescriptionItemRepository.UpdateAsync(existingPrescriptionItem);


        // ← تسجيل نجاح التحديث
        var changes = new List<string>();
        if (oldMedicationId != prescriptionItemDto.MedicationId)
            changes.Add($"MedicationId: {oldMedicationId} -> {prescriptionItemDto.MedicationId}");
        if (oldCustomName != prescriptionItemDto.CustomMedicationName)
            changes.Add($"CustomMedicationName: {oldCustomName} -> {prescriptionItemDto.CustomMedicationName}");
        if (oldDosage != prescriptionItemDto.Dosage)
            changes.Add($"Dosage: {oldDosage} -> {prescriptionItemDto.Dosage}");

        await _auditService.LogAsync(
            action: "UpdatePrescriptionItem",
            entityName: "PrescriptionItem",
            entityId: id.ToString(),
            details: $"PrescriptionItem updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No significant changes")}");


        return new PrescriptionItemDto
        {
            Id = PrescriptionItem.Id,
            PrescriptionId = PrescriptionItem.PrescriptionId,
            MedicationId = PrescriptionItem.MedicationId,
            Dosage = PrescriptionItem.Dosage,
            Frequency = PrescriptionItem.Frequency,
            Duration = PrescriptionItem.Duration,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingItem = await _prescriptionItemRepository.GetByIdAsync(id);

        if (existingItem == null)
        {
            // ← تسجيل فشل الحذف (العنصر غير موجود)
            await _auditService.LogAsync(
                action: "DeletePrescriptionItemFailed",
                entityName: "PrescriptionItem",
                entityId: id.ToString(),
                details: $"Failed to delete PrescriptionItem - Item with ID {id} not found");

            return false;
        }

        var result = await _prescriptionItemRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeletePrescriptionItem",
                entityName: "PrescriptionItem",
                entityId: id.ToString(),
                details: $"PrescriptionItem deleted successfully - PrescriptionId: {existingItem.PrescriptionId}");
        }

        return result;
    }

    public async Task<List<PrescriptionItemDto>> GetAllByPrescriptionId(int prescriptionId)
    {
        var prescriptionItems = await _prescriptionItemRepository.GetAllAsync();

        return prescriptionItems.Where(p => p.PrescriptionId == prescriptionId).Select(pi => new PrescriptionItemDto
        {
            Id = pi.Id,
            PrescriptionId = pi.PrescriptionId,
            MedicationId = pi.MedicationId,
            Dosage = pi.Dosage,
            Frequency = pi.Frequency,
            Duration = pi.Duration,
            //Medication = pi.Medication,
            // Prescription = pi.Prescription,
        }).ToList();
    }
}