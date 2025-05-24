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

public class PharmacistService : IPharmacistService
{
    private readonly IPharmacistRepository _pharmacistRepository;

    public PharmacistService(IPharmacistRepository pharmacistRepository)
    {
        _pharmacistRepository = pharmacistRepository;
    }

    public async Task<PharmacistDto?> GetByIdAsync(int id)
    {
        var pharmacist = await _pharmacistRepository.GetByIdAsync(id);
        if (pharmacist == null) return null;

        return new PharmacistDto
        {
            Id = pharmacist.Id,
            UserId = pharmacist.UserId,
            PharmacyId = pharmacist.PharmacyId,
            LicenseNumber = pharmacist.LicenseNumber,
            DispenseRecords = pharmacist.DispenseRecords.Select(dr => new DispenseRecordDto
            {
                Id = dr.Id,
                PrescriptionId = dr.PrescriptionId,
                PharmacistId = dr.PharmacistId,
                PharmacyId = dr.PharmacyId,
                DispensedDate = dr.DispensedDate
            }).ToList(),
            User = new UserDto
            {
                Id = pharmacist.User.Id,
                FullName = pharmacist.User.FullName,
                Email = pharmacist.User.Email,
                Role = (UserRoleEnum)pharmacist.User.RoleId,
                CreatedAt = pharmacist.User.CreatedAt,


            },
            Pharmacy = new PharmacyDto
            {
                Id = pharmacist.Pharmacy.Id,
                Name = pharmacist.Pharmacy.Name,
                Address = pharmacist.Pharmacy.Address,
                Phone = pharmacist.Pharmacy.Phone,
            }
        };
    }

    public async Task<List<PharmacistDto>> GetAllAsync()
    {
        var pharmacists = await _pharmacistRepository.GetAllAsync();
        return pharmacists
            .Select(p => new PharmacistDto
        {
            Id = p.Id,
            UserId = p.UserId,
            PharmacyId = p.PharmacyId,
            LicenseNumber = p.LicenseNumber,
                DispenseRecords = p.DispenseRecords.Select(dr => new DispenseRecordDto
                {
                    Id = dr.Id,
                    PrescriptionId = dr.PrescriptionId,
                    PharmacistId = dr.PharmacistId,
                    PharmacyId = dr.PharmacyId,
                    DispensedDate = dr.DispensedDate
                }).ToList(),
                User = new UserDto
                {
                    Id = p.User.Id,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    Role = (UserRoleEnum)p.User.RoleId,
                    CreatedAt = p.User.CreatedAt,


                },
                Pharmacy = new PharmacyDto
                {
                    Id = p.Pharmacy.Id,
                    Name = p.Pharmacy.Name,
                    Address = p.Pharmacy.Address,
                    Phone = p.Pharmacy.Phone,
                }
            }).ToList();

          
    }

    public async Task<PharmacistDto> CreateAsync(CreatePharmacistDto pharmacistDto)
    {
        var pharmacist = new Pharmacist
        {
            UserId = pharmacistDto.UserId,
            PharmacyId = pharmacistDto.PharmacyId,
            LicenseNumber = pharmacistDto.LicenseNumber,
        };

        var addedPharmacist = await _pharmacistRepository.AddAsync(pharmacist);
        return new PharmacistDto
        {
            Id = addedPharmacist.Id,
            UserId = addedPharmacist.UserId,
            PharmacyId = addedPharmacist.PharmacyId,
            LicenseNumber = addedPharmacist.LicenseNumber,
        };
    }

    public async Task<PharmacistDto> UpdateAsync(int id, UpdatePharmacistDto pharmacistDto)
    {
        var existingPharmacist = await _pharmacistRepository.GetByIdAsync(id);
        if (existingPharmacist == null) return null;

      //  existingPharmacist.UserId = existingPharmacist.UserId;
        existingPharmacist.LicenseNumber = pharmacistDto.LicenseNumber;
        existingPharmacist.PharmacyId = pharmacistDto.PharmacyId;
        Pharmacist Pharmacist  = await _pharmacistRepository.UpdateAsync(existingPharmacist);
        return new PharmacistDto
        {
            Id = Pharmacist.Id,
            UserId = Pharmacist.UserId,
            PharmacyId = Pharmacist.PharmacyId,
            LicenseNumber = existingPharmacist.LicenseNumber,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
       return await _pharmacistRepository.DeleteAsync(id);
    }

    public async Task<List<PharmacistDto>> GetByPharmacyIdAsync(int PharmacyId)
    {
        var pharmacists = await _pharmacistRepository.GetByPharmacyIdAsync(PharmacyId);
        return pharmacists
            .Select(p => new PharmacistDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PharmacyId = p.PharmacyId,
                LicenseNumber = p.LicenseNumber,
                DispenseRecords = p.DispenseRecords.Select(dr => new DispenseRecordDto
                {
                    Id = dr.Id,
                    PrescriptionId = dr.PrescriptionId,
                    PharmacistId = dr.PharmacistId,
                    PharmacyId = dr.PharmacyId,
                    DispensedDate = dr.DispensedDate
                }).ToList(),
                User = new UserDto
                {
                    Id = p.User.Id,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    Role = (UserRoleEnum)p.User.RoleId,
                    CreatedAt = p.User.CreatedAt,


                },
                Pharmacy = new PharmacyDto
                {
                    Id = p.Pharmacy.Id,
                    Name = p.Pharmacy.Name,
                    Address = p.Pharmacy.Address,
                    Phone = p.Pharmacy.Phone,
                }
            }).ToList();
    }

    public async Task<PharmacistDto> GetPharmacyByUserIdAsync(int userId)
    {
        var pharmacist = await _pharmacistRepository.GetPharmacyByUserIdAsync(userId);
        if (pharmacist == null) return null;

        return new PharmacistDto
        {
            Id = pharmacist.Id,
            UserId = pharmacist.UserId,
            PharmacyId = pharmacist.PharmacyId,
            LicenseNumber = pharmacist.LicenseNumber,
            DispenseRecords = pharmacist.DispenseRecords.Select(dr => new DispenseRecordDto
            {
                Id = dr.Id,
                PrescriptionId = dr.PrescriptionId,
                PharmacistId = dr.PharmacistId,
                PharmacyId = dr.PharmacyId,
                DispensedDate = dr.DispensedDate
            }).ToList(),
            User = new UserDto
            {
                Id = pharmacist.User.Id,
                FullName = pharmacist.User.FullName,
                Email = pharmacist.User.Email,
                Role = (UserRoleEnum)pharmacist.User.RoleId,
                CreatedAt = pharmacist.User.CreatedAt,


            },
            Pharmacy = new PharmacyDto
            {
                Id = pharmacist.Pharmacy.Id,
                Name = pharmacist.Pharmacy.Name,
                Address = pharmacist.Pharmacy.Address,
                Phone = pharmacist.Pharmacy.Phone,
            }
        };
    }

    public async Task<PharmacistDashboardStatsDto> GetPharmacistDataAsync(int PharmacistId)
    {
        var state = await _pharmacistRepository.GetPharmacistDataAsync(PharmacistId);
        if (state == null) return null;

        return state;
    }
}