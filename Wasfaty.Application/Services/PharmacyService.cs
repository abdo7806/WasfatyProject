using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.Application.Services;

public class PharmacyService : IPharmacyService
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IAuditService _auditService;
    public PharmacyService(IPharmacyRepository pharmacyRepository, IAuditService auditService)
    {
        _pharmacyRepository = pharmacyRepository;
        _auditService = auditService;
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
        // التحقق من وجود صيدلية بنفس الاسم
        var existingPharmacies = await _pharmacyRepository.GetAllAsync();
        if (existingPharmacies.Any(p => p.Name == pharmacyDto.Name))
        {
            // ← تسجيل فشل الإنشاء (اسم مكرر)
            await _auditService.LogAsync(
                action: "CreatePharmacyFailed",
                entityName: "Pharmacy",
                details: $"Failed to create Pharmacy - Name already exists: {pharmacyDto.Name}");

            return null;
        }

        var pharmacy = new Pharmacy
        {
            Name = pharmacyDto.Name,
            Address = pharmacyDto.Address,
            Phone = pharmacyDto.Phone,
        };

        var addedPharmacy = await _pharmacyRepository.AddAsync(pharmacy);

        if (addedPharmacy != null)
        {
            // ← تسجيل نجاح الإنشاء
            await _auditService.LogAsync(
                action: "CreatePharmacy",
                entityName: "Pharmacy",
                entityId: addedPharmacy.Id.ToString(),
                details: $"Pharmacy created successfully - Name: {pharmacyDto.Name}, Address: {pharmacyDto.Address}, Phone: {pharmacyDto.Phone}");
        }
        else
        {
            // ← تسجيل فشل الإنشاء (خطأ غير متوقع)
            await _auditService.LogAsync(
                action: "CreatePharmacyFailed",
                entityName: "Pharmacy",
                details: $"Failed to create Pharmacy - Unknown error for Name: {pharmacyDto.Name}");

            return null;
        }

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
        if (existingPharmacy == null)
        {
            // ← تسجيل فشل التحديث (الصيدلية غير موجودة)
            await _auditService.LogAsync(
                action: "UpdatePharmacyFailed",
                entityName: "Pharmacy",
                entityId: id.ToString(),
                details: $"Failed to update Pharmacy - Pharmacy with ID {id} not found");

            return null;
        }

        // حفظ القيم القديمة للتسجيل
        var oldName = existingPharmacy.Name;
        var oldAddress = existingPharmacy.Address;
        var oldPhone = existingPharmacy.Phone;

        // التحقق من عدم تكرار الاسم (إذا تم تغيير الاسم)
        if (oldName != pharmacyDto.Name)
        {
            var existingPharmacies = await _pharmacyRepository.GetAllAsync();
            if (existingPharmacies.Any(p => p.Name == pharmacyDto.Name && p.Id != id))
            {
                // ← تسجيل فشل التحديث (اسم مكرر)
                await _auditService.LogAsync(
                    action: "UpdatePharmacyFailed",
                    entityName: "Pharmacy",
                    entityId: id.ToString(),
                    details: $"Failed to update Pharmacy - Name already exists: {pharmacyDto.Name}");

                return null;
            }
        }

        existingPharmacy.Name = pharmacyDto.Name;
        existingPharmacy.Address = pharmacyDto.Address;
        existingPharmacy.Phone = pharmacyDto.Phone;

        await _pharmacyRepository.UpdateAsync(existingPharmacy);

        var changes = new List<string>();
        if (oldName != pharmacyDto.Name) changes.Add($"Name: {oldName} -> {pharmacyDto.Name}");
        if (oldAddress != pharmacyDto.Address) changes.Add($"Address: {oldAddress} -> {pharmacyDto.Address}");
        if (oldPhone != pharmacyDto.Phone) changes.Add($"Phone: {oldPhone} -> {pharmacyDto.Phone}");

        await _auditService.LogAsync(
            action: "UpdatePharmacy",
            entityName: "Pharmacy",
            entityId: id.ToString(),
            details: $"Pharmacy updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

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
        var existingPharmacy = await _pharmacyRepository.GetByIdAsync(id);

        if (existingPharmacy == null)
        {
            // ← تسجيل فشل الحذف (الصيدلية غير موجودة)
            await _auditService.LogAsync(
                action: "DeletePharmacyFailed",
                entityName: "Pharmacy",
                entityId: id.ToString(),
                details: $"Failed to delete Pharmacy - Pharmacy with ID {id} not found");

            return false;
        }

        var pharmacyName = existingPharmacy.Name;

        // التحقق من وجود صيادلة مرتبطين بهذه الصيدلية
        if (existingPharmacy.Pharmacists != null && existingPharmacy.Pharmacists.Any())
        {
            // ← تسجيل فشل الحذف (يوجد صيادلة مرتبطون)
            await _auditService.LogAsync(
                action: "DeletePharmacyFailed",
                entityName: "Pharmacy",
                entityId: id.ToString(),
                details: $"Failed to delete Pharmacy - Pharmacy has {existingPharmacy.Pharmacists.Count} associated pharmacists. Name: {pharmacyName}");

            return false;
        }

        var result = await _pharmacyRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeletePharmacy",
                entityName: "Pharmacy",
    
                entityId: id.ToString(),
                details: $"Pharmacy deleted successfully - Name: {pharmacyName}");
        }

        return result;
    }
}
