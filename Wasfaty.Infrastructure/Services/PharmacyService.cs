using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;


public class PharmacyService : IPharmacyService
{
    private readonly IPharmacyRepository _pharmacyRepository;

    public PharmacyService(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }


    public async Task<IEnumerable<PharmacyDto>> GetAllAsync()
    {
      //  Pharmacy
        var pharmacies = await _pharmacyRepository.GetAllAsync();
        return pharmacies.Select(p => new PharmacyDto
        {
            Id = p.Id,
            Name = p.Name,
            Address = p.Address,
            Phone = p.Phone,
            DispenseRecords = p.DispenseRecords.Select(dr => new DispenseRecordDto
            {
                Id = dr.Id,
                PrescriptionId = dr.PrescriptionId,
                PharmacistId = dr.PharmacistId,
                PharmacyId = dr.PharmacyId,
                DispensedDate = dr.DispensedDate

            }).ToList(),
            Pharmacists = p.Pharmacists.Select(ph => new PharmacistDto 
            {
                Id = ph.Id,
                UserId = ph.UserId,
                PharmacyId = ph.PharmacyId,
                LicenseNumber = ph.LicenseNumber,
                DispenseRecords = ph.DispenseRecords.Select(dr => new DispenseRecordDto
                {
                    Id = dr.Id,
                    PrescriptionId = dr.PrescriptionId,
                    PharmacistId = dr.PharmacistId,
                    PharmacyId = dr.PharmacyId,
                    DispensedDate = dr.DispensedDate
                }).ToList(),

                User = new UserDto
                {
                    Id = ph.User.Id,
                    FullName = ph.User.FullName,
                    Email = ph.User.Email,
                    Role = (UserRoleEnum)ph.User.RoleId,
                    CreatedAt = ph.User.CreatedAt,


                },
                Pharmacy = new PharmacyDto
                {
                    Id = ph.Pharmacy.Id,
                    Name = ph.Pharmacy.Name,
                    Address = ph.Pharmacy.Address,
                    Phone = ph.Pharmacy.Phone,
                }
            }).ToList(),
     
        }).ToList();
    }

    public async Task<PharmacyDto> CreateAsync(CreatePharmacyDto pharmacyDto)
    {
        var pharmacy = new Pharmacy
        {
            Name = pharmacyDto.Name,
            Address = pharmacyDto.Address,
            Phone = pharmacyDto.Phone,
        };

        var addedPharmacy = await _pharmacyRepository.AddAsync(pharmacy);
        return new PharmacyDto
        {
            Id = addedPharmacy.Id,
            Name = addedPharmacy.Name,
            Address = addedPharmacy.Address,
            Phone = addedPharmacy.Phone,
        };
    }

    public async Task<PharmacyDto?> GetByIdAsync(int id)
    {
        var pharmacy = await _pharmacyRepository.GetByIdAsync(id);
        if (pharmacy == null) return null;

        return new PharmacyDto
        {
            Id = pharmacy.Id,
            Name = pharmacy.Name,
            Address = pharmacy.Address,
            Phone = pharmacy.Phone,
            DispenseRecords = pharmacy.DispenseRecords.Select(dr => new DispenseRecordDto
            {
                Id = dr.Id,
                PrescriptionId = dr.PrescriptionId,
                PharmacistId = dr.PharmacistId,
                PharmacyId = dr.PharmacyId,
                DispensedDate = dr.DispensedDate
             
            }).ToList(),
         
            Pharmacists = pharmacy.Pharmacists.Select(ph => new PharmacistDto
            {
                Id = ph.Id,
                UserId = ph.UserId,
                PharmacyId = ph.PharmacyId,
                LicenseNumber = ph.LicenseNumber,
                DispenseRecords = ph.DispenseRecords.Select(dr => new DispenseRecordDto
                {
                    Id = dr.Id,
                    PrescriptionId = dr.PrescriptionId,
                    PharmacistId = dr.PharmacistId,
                    PharmacyId = dr.PharmacyId,
                    DispensedDate = dr.DispensedDate
                }).ToList(),

                User = new UserDto
                {
                    Id = ph.User.Id,
                    FullName = ph.User.FullName,
                    Email = ph.User.Email,
                    Role = (UserRoleEnum)ph.User.RoleId,
                    CreatedAt = ph.User.CreatedAt,


                },
                Pharmacy = new PharmacyDto
                {
                    Id = ph.Pharmacy.Id,
                    Name = ph.Pharmacy.Name,
                    Address = ph.Pharmacy.Address,
                    Phone = ph.Pharmacy.Phone,
                }
            }).ToList(),

        };
    }


    public async Task<PharmacyDto> UpdateAsync(int id, UpdatePharmacyDto pharmacyDto)
    {
        var existingPharmacy = await _pharmacyRepository.GetByIdAsync(id);
        if (existingPharmacy == null) return null;

        existingPharmacy.Name = pharmacyDto.Name;
        existingPharmacy.Address = pharmacyDto.Address;
        existingPharmacy.Phone = pharmacyDto.Phone;

        await _pharmacyRepository.UpdateAsync(existingPharmacy);

        return new PharmacyDto
        {
            Id = id,
            Name = pharmacyDto.Name,
            Address = pharmacyDto.Address,
            Phone = pharmacyDto.Phone,
        };
    }


    public async Task<bool> DeleteAsync(int id)
    {
        return await _pharmacyRepository.DeleteAsync(id);
    }
}
