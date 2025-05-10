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

    public PrescriptionItemService(IPrescriptionItemRepository prescriptionItemRepository)
    {
        _prescriptionItemRepository = prescriptionItemRepository;
    }


    public async Task<PrescriptionItemDto> GetByIdAsync(int id)
    {
        var prescriptionItem = await _prescriptionItemRepository.GetByIdAsync(id);
        if (prescriptionItem == null) return null;

        return new PrescriptionItemDto
        {
            Id = prescriptionItem.Id,
            PrescriptionId = prescriptionItem.PrescriptionId,
            MedicationId = prescriptionItem.MedicationId,
            Dosage = prescriptionItem.Dosage,
            Frequency = prescriptionItem.Frequency,
            Duration = prescriptionItem.Duration,
            Medication = new MedicationDto
            {
                Id = prescriptionItem.Medication.Id,
                Name = prescriptionItem.Medication.Name,
                Description = prescriptionItem.Medication.Description,
                DosageForm = prescriptionItem.Medication.DosageForm,
                Strength = prescriptionItem.Medication.Strength,
            },
            Prescription = new PrescriptionDto
            {
                Id = prescriptionItem.Prescription.Id,
                DoctorId = prescriptionItem.Prescription.DoctorId,
                PatientId = prescriptionItem.Prescription.PatientId,
                IssuedDate = prescriptionItem.Prescription.IssuedDate,
                IsDispensed = prescriptionItem.Prescription.IsDispensed,

            }
        };
    }


    public async Task<List<PrescriptionItemDto>> GetAllAsync()
    {
        var prescriptionItems = await _prescriptionItemRepository.GetAllAsync();
        return prescriptionItems.Select(pi => new PrescriptionItemDto
        {
            Id = pi.Id,
            PrescriptionId = pi.PrescriptionId,
            MedicationId = pi.MedicationId,
            Dosage = pi.Dosage,
            Frequency = pi.Frequency,
            Duration = pi.Duration,
            Medication = new MedicationDto
            {
                Id = pi.Medication.Id,
                Name = pi.Medication.Name,
                Description = pi.Medication.Description,
                DosageForm = pi.Medication.DosageForm,
                Strength = pi.Medication.Strength,
            },
            Prescription = new PrescriptionDto
            {
                Id = pi.Prescription.Id,
                DoctorId = pi.Prescription.DoctorId,
                PatientId = pi.Prescription.PatientId,
                IssuedDate = pi.Prescription.IssuedDate,
                IsDispensed = pi.Prescription.IsDispensed,
                
            }
        
        }).ToList();
    }

    public async Task<PrescriptionItemDto> CreateAsync(CreatePrescriptionItemDto prescriptionItemDto)
    {
        var prescriptionItem = new PrescriptionItem
        {
            PrescriptionId = prescriptionItemDto.PrescriptionId,
            MedicationId = prescriptionItemDto.MedicationId,
            Dosage = prescriptionItemDto.Dosage,
            Frequency = prescriptionItemDto.Frequency,
            Duration = prescriptionItemDto.Duration,
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

        existingPrescriptionItem.MedicationId = prescriptionItemDto.MedicationId;
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