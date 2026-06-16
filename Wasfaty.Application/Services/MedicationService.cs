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

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IAuditService _auditService;

    public MedicationService(IMedicationRepository medicationRepository, IAuditService auditService)
    {
        _medicationRepository = medicationRepository;
        _auditService = auditService;
    }

    public async Task<MedicationDto?> GetByIdAsync(int id)
    {

        var medication = await _medicationRepository.GetByIdAsync(id);
        if (medication == null) return null;

        return new MedicationDto
        {
            Id = medication.Id,
            Name = medication.Name,
            Description = medication.Description,
            DosageForm = medication.DosageForm,
            Strength = medication.Strength,
            PrescriptionItems = medication.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,
                //Medication = pi.Medication,
               // Prescription = pi.Prescription,
            }).ToList(),
        };
    }

    public async Task<List<MedicationDto>> GetAllAsync()
    {
        var medications = await _medicationRepository.GetAllAsync();
        return medications.Select(m => new MedicationDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            DosageForm = m.DosageForm,
            Strength = m.Strength,
            PrescriptionItems = m.PrescriptionItems.Select(pi => new PrescriptionItemDto
            {
                Id = pi.Id,
                PrescriptionId = pi.PrescriptionId,
                MedicationId = pi.MedicationId,
                Dosage = pi.Dosage,
                Frequency = pi.Frequency,
                Duration = pi.Duration,
               // Medication = pi.Medication,
                //Prescription = pi.Prescription,
            }).ToList(),
        }).ToList();
    
    }

    public async Task<MedicationDto> CreateAsync(CreateMedicationDto medicationDto)
    {
        // التحقق من وجود دواء بنفس الاسم
        var existingMedications = await _medicationRepository.GetAllAsync();
        if (existingMedications.Any(m => m.Name == medicationDto.Name))
        {
            // ← تسجيل فشل الإنشاء (اسم مكرر)
            await _auditService.LogAsync(
                action: "CreateMedicationFailed",
                entityName: "Medication",
                details: $"Failed to create Medication - Name already exists: {medicationDto.Name}");

            return null;
        }

        var medication = new Medication
        {
            Name = medicationDto.Name,
            Description = medicationDto.Description,
            DosageForm = medicationDto.DosageForm,
            Strength = medicationDto.Strength,
        };

        var addedMedication = await _medicationRepository.AddAsync(medication);

        if (addedMedication != null)
        {
            // ← تسجيل نجاح الإنشاء
            await _auditService.LogAsync(
                action: "CreateMedication",
                entityName: "Medication",
                entityId: addedMedication.Id.ToString(),
                details: $"Medication created successfully - Name: {medicationDto.Name}, DosageForm: {medicationDto.DosageForm}, Strength: {medicationDto.Strength}");
        }
        else
        {
            // ← تسجيل فشل الإنشاء (خطأ غير متوقع)
            await _auditService.LogAsync(
                action: "CreateMedicationFailed",
                entityName: "Medication",
                details: $"Failed to create Medication - Unknown error for Name: {medicationDto.Name}");

            return null;
        }

        return new MedicationDto
        {
            Id = addedMedication.Id,
            Name = addedMedication.Name,
            Description = addedMedication.Description,
            DosageForm = addedMedication.DosageForm,
            Strength = addedMedication.Strength,
        };
    }

    public async Task<MedicationDto> UpdateAsync(int id, UpdateMedicationDto medicationDto)
    {
        var existingMedication = await _medicationRepository.GetByIdAsync(id);

        if (existingMedication == null)
        {
            // ← تسجيل فشل التحديث (الدواء غير موجود)
            await _auditService.LogAsync(
                action: "UpdateMedicationFailed",
                entityName: "Medication",
                entityId: id.ToString(),
                details: $"Failed to update Medication - Medication with ID {id} not found");

            return null;
        }

        // حفظ القيم القديمة للتسجيل
        var oldName = existingMedication.Name;
        var oldDescription = existingMedication.Description;
        var oldDosageForm = existingMedication.DosageForm;
        var oldStrength = existingMedication.Strength;

        existingMedication.Name = medicationDto.Name;
        existingMedication.Description = medicationDto.Description;
        existingMedication.DosageForm = medicationDto.DosageForm;
        existingMedication.Strength = medicationDto.Strength;

        Medication medication = await _medicationRepository.UpdateAsync(existingMedication);

        var changes = new List<string>();
        if (oldName != medicationDto.Name) changes.Add($"Name: {oldName} -> {medicationDto.Name}");
        if (oldDescription != medicationDto.Description) changes.Add($"Description: {oldDescription} -> {medicationDto.Description}");
        if (oldDosageForm != medicationDto.DosageForm) changes.Add($"DosageForm: {oldDosageForm} -> {medicationDto.DosageForm}");
        if (oldStrength != medicationDto.Strength) changes.Add($"Strength: {oldStrength} -> {medicationDto.Strength}");

        await _auditService.LogAsync(
            action: "UpdateMedication",
            entityName: "Medication",
            entityId: id.ToString(),
            details: $"Medication updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

        return new MedicationDto
        {
            Id = medication.Id,
            Name = medication.Name,
            Description = medication.Description,
            DosageForm = medication.DosageForm,
            Strength = medication.Strength,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingMedication = await _medicationRepository.GetByIdAsync(id);

        if (existingMedication == null)
        {
            // ← تسجيل فشل الحذف (الدواء غير موجود)
            await _auditService.LogAsync(
                action: "DeleteMedicationFailed",
                entityName: "Medication",
                entityId: id.ToString(),
                details: $"Failed to delete Medication - Medication with ID {id} not found");

            return false;
        }

        var medicationName = existingMedication.Name;
        var result = await _medicationRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeleteMedication",
                entityName: "Medication",
                entityId: id.ToString(),
                details: $"Medication deleted successfully - Name: {medicationName}");
        }

        return result;
    }

    public async Task<List<MedicationDto>> GetMedicationsByIdsAsync(List<int> ids)
    {
        try
        {
            if (ids == null || !ids.Any())
                return null;


            var medications = await _medicationRepository.GetMedicationsByIdsAsync(ids);
            return medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                PrescriptionItems = m.PrescriptionItems.Select(pi => new PrescriptionItemDto
                {
                    Id = pi.Id,
                    PrescriptionId = pi.PrescriptionId,
                    MedicationId = pi.MedicationId,
                    Dosage = pi.Dosage,
                    Frequency = pi.Frequency,
                    Duration = pi.Duration,
                    // Medication = pi.Medication,
                    //Prescription = pi.Prescription,
                }).ToList(),
            }).ToList();


          
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}