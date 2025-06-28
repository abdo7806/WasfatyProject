using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Medications;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces;

public class PrescriptionItemService : IPrescriptionItemService
{
    private readonly IPrescriptionItemRepository _prescriptionItemRepository;
    private readonly IMedicationRepository _medicationRepository;
    public PrescriptionItemService(IPrescriptionItemRepository prescriptionItemRepository, IMedicationRepository medicationRepository)
    {
        _prescriptionItemRepository = prescriptionItemRepository;
        _medicationRepository = medicationRepository;
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
            throw new ArgumentException("يجب إدخال إما دواء موجود أو بيانات دواء مخصص");
        }

        // إذا كان دواءً موجوداً، التحقق من وجوده
        if (prescriptionItemDto.MedicationId.HasValue)
        {
            var medicationExists = await _medicationRepository.GetAllAsync();
            if (medicationExists == null)
            {
                throw new KeyNotFoundException("الدواء المحدد غير موجود");
            }
        }

        var prescriptionItem = new PrescriptionItem
        {
            PrescriptionId = prescriptionItemDto.PrescriptionId,
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
            throw new ArgumentException("يجب إدخال إما دواء موجود أو بيانات دواء مخصص");
        }

        // إذا كان دواءً موجوداً، التحقق من وجوده
        if (prescriptionItemDto.MedicationId.HasValue)
        {
            var medicationExists = await _medicationRepository.GetAllAsync();
            if (medicationExists == null)
            {
                throw new KeyNotFoundException("الدواء المحدد غير موجود");
            }
        }




        existingPrescriptionItem.MedicationId = prescriptionItemDto.MedicationId;
        existingPrescriptionItem.CustomMedicationName = prescriptionItemDto.CustomMedicationName;
        existingPrescriptionItem.CustomMedicationDescription = prescriptionItemDto.CustomMedicationDescription;
        existingPrescriptionItem.CustomDosageForm = prescriptionItemDto.CustomDosageForm;
        existingPrescriptionItem.CustomStrength = prescriptionItemDto.CustomStrength;
        existingPrescriptionItem.Dosage = prescriptionItemDto.Dosage;
        existingPrescriptionItem.Frequency = prescriptionItemDto.Frequency;
        existingPrescriptionItem.Duration = prescriptionItemDto.Duration;

        PrescriptionItem PrescriptionItem = await _prescriptionItemRepository.UpdateAsync(existingPrescriptionItem);
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
        return await _prescriptionItemRepository.DeleteAsync(id);
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