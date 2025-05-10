using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Medications;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.Interfaces;

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _medicationRepository;

    public MedicationService(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository;
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
        var medication = new Medication
        {
            Name = medicationDto.Name,
            Description = medicationDto.Description,
            DosageForm = medicationDto.DosageForm,
            Strength = medicationDto.Strength,
        };

        var addedMedication = await _medicationRepository.AddAsync(medication);
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
        if (existingMedication == null) return null;

        existingMedication.Name = medicationDto.Name;
        existingMedication.Description = medicationDto.Description;
        existingMedication.DosageForm = medicationDto.DosageForm;
        existingMedication.Strength = medicationDto.Strength;

        Medication medication = await _medicationRepository.UpdateAsync(existingMedication);
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
       return await _medicationRepository.DeleteAsync(id);
    }
}