using System.Net;
using System.Numerics;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;
namespace Wasfaty.Application.Services;

public class MedicalCenterService : IMedicalCenterService
{
    private readonly IMedicalCenterRepository _medicalCenterRepository;
    private readonly IAuditService _auditService;

    public MedicalCenterService(IMedicalCenterRepository medicalCenterRepository, IAuditService auditService)
    {
        _medicalCenterRepository = medicalCenterRepository;
        _auditService = auditService;
    }

    public async Task<IEnumerable<MedicalCenterDto>> GetAllAsync()
    {
        IEnumerable<MedicalCenter> medicalCenters = await _medicalCenterRepository.GetAllAsync();

        return medicalCenters.Select(x => new MedicalCenterDto
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            Phone = x.Phone,
        }
        );
    }

    public async Task<MedicalCenterDto?> GetByIdAsync(int id)
    {
        var medicalCenter = await _medicalCenterRepository.GetByIdAsync(id);

        if (medicalCenter == null)
        {
            return null;
        }

        return new MedicalCenterDto
        {
            Id = medicalCenter.Id,
            Name = medicalCenter.Name,
            Address = medicalCenter.Address,
            Phone = medicalCenter.Phone,
        };
    }


    public async Task<MedicalCenterDto> CreateAsync(CreateMedicalCenterDto medicalCenterDto)
    {

        // التحقق من وجود مركز طبي بنفس الاسم (اختياري)
        var existingCenters = await _medicalCenterRepository.GetAllAsync();
        if (existingCenters.Any(x => x.Name == medicalCenterDto.Name))
        {
            // ← تسجيل فشل الإنشاء (اسم مكرر)
            await _auditService.LogAsync(
                action: "CreateMedicalCenterFailed",
                entityName: "MedicalCenter",
                details: $"Failed to create MedicalCenter - Name already exists: {medicalCenterDto.Name}");

            return null;
        }

        var medicalCenter = new MedicalCenter
        {
           
            Name = medicalCenterDto.Name,
            Address = medicalCenterDto.Address,
            Phone = medicalCenterDto.Phone,
        };


        MedicalCenter createdMedicalCenter = await _medicalCenterRepository.AddAsync(medicalCenter);

        if(createdMedicalCenter != null)
        {
            await _auditService.LogAsync(
    action: "CreateMedicalCenter",
    entityName: "MedicalCenter",
    entityId: createdMedicalCenter.Id.ToString(),
    details: $"MedicalCenter created successfully - Name: {medicalCenterDto.Name}, Address: {medicalCenterDto.Address}, Phone: {medicalCenterDto.Phone}");


            return new MedicalCenterDto
            {
                Id = createdMedicalCenter.Id,
                Name = createdMedicalCenter.Name,
                Address = createdMedicalCenter.Address ?? "",
                Phone = createdMedicalCenter.Phone ?? "",
            };
        }

        // ← تسجيل فشل الإنشاء (خطأ غير متوقع)
        await _auditService.LogAsync(
            action: "CreateMedicalCenterFailed",
            entityName: "MedicalCenter",
            details: $"Failed to create MedicalCenter - Unknown error for Name: {medicalCenterDto.Name}");

        return null;
    }

    public async Task<MedicalCenterDto> UpdateAsync(int id, UpdateMedicalCenterDto medicalCenterDto)
    {

        // احصل على المركز الطبي الحالي
        var existingMedicalCenter = await _medicalCenterRepository.GetByIdAsync(id);

        if (existingMedicalCenter == null)
        {
            // ← تسجيل فشل التحديث (المركز غير موجود)
            await _auditService.LogAsync(
                action: "UpdateMedicalCenterFailed",
                entityName: "MedicalCenter",
                entityId: id.ToString(),
                details: $"Failed to update MedicalCenter - Center with ID {id} not found");

            return null;
        }

        // حفظ القيم القديمة للتسجيل
        var oldName = existingMedicalCenter.Name;
        var oldAddress = existingMedicalCenter.Address;
        var oldPhone = existingMedicalCenter.Phone;


        // تحديث الخصائص مباشرة
        existingMedicalCenter.Name = medicalCenterDto.Name;
        existingMedicalCenter.Address = medicalCenterDto.Address;
        existingMedicalCenter.Phone = medicalCenterDto.Phone;

        // تحديث الكائن في قاعدة البيانات
        await _medicalCenterRepository.UpdateAsync(existingMedicalCenter);

        // ← تسجيل نجاح التحديث
        var changes = new List<string>();
        if (oldName != medicalCenterDto.Name) changes.Add($"Name: {oldName} -> {medicalCenterDto.Name}");
        if (oldAddress != medicalCenterDto.Address) changes.Add($"Address: {oldAddress} -> {medicalCenterDto.Address}");
        if (oldPhone != medicalCenterDto.Phone) changes.Add($"Phone: {oldPhone} -> {medicalCenterDto.Phone}");

        await _auditService.LogAsync(
            action: "UpdateMedicalCenter",
            entityName: "MedicalCenter",
            entityId: id.ToString(),
            details: $"MedicalCenter updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

        // قم بإرجاع DTO بعد التحديث
        return new MedicalCenterDto
        {
            Id = existingMedicalCenter.Id,
            Name = existingMedicalCenter.Name,
            Address = existingMedicalCenter.Address,
            Phone = existingMedicalCenter.Phone,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingMedicalCenter = await _medicalCenterRepository.GetByIdAsync(id);

        if (existingMedicalCenter == null)
        {
            // ← تسجيل فشل الحذف (المركز غير موجود)
            await _auditService.LogAsync(
                action: "DeleteMedicalCenterFailed",
                entityName: "MedicalCenter",
                entityId: id.ToString(),
                details: $"Failed to delete MedicalCenter - Center with ID {id} not found");

            return false;
        }

        var centerName = existingMedicalCenter.Name;
        var result = await _medicalCenterRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeleteMedicalCenter",
                entityName: "MedicalCenter",
                entityId: id.ToString(),
                details: $"MedicalCenter deleted successfully - Name: {centerName}");
        }

        return result;
    }
}