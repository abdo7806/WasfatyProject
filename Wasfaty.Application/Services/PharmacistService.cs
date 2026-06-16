using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.DTOs.Pharmacists;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;


namespace Wasfaty.Application.Services;

public class PharmacistService : IPharmacistService
{
    private readonly IPharmacistRepository _pharmacistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public PharmacistService(IPharmacistRepository pharmacistRepository, 
        IUnitOfWork unitOfWork, IUserRepository userRepository, 
        IAuditService auditService)
    {
        _pharmacistRepository = pharmacistRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _auditService = auditService;
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
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // التحقق من وجود البريد الإلكتروني مسبقاً
            var existingUser = await _userRepository.GetByEmailAsync(pharmacistDto.Email);
            if (existingUser != null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (بريد موجود)
                await _auditService.LogAsync(
                    action: "CreatePharmacistFailed",
                    entityName: "Pharmacist",
                    details: $"Failed to create pharmacist - Email already exists: {pharmacistDto.Email}");

                return null;
            }

            var user = new User
            {
                FullName = pharmacistDto.FullName,
                Email = pharmacistDto.Email,
                RoleId = (int)UserRoleEnum.Pharmacist,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(pharmacistDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);
            if (createdUser == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء المستخدم)
                await _auditService.LogAsync(
                    action: "CreatePharmacistFailed",
                    entityName: "Pharmacist",
                    details: $"Failed to create pharmacist - User creation failed for email: {pharmacistDto.Email}");

                return null;
            }


            var pharmacist = new Pharmacist
            {
                UserId = createdUser.Id,
                PharmacyId = pharmacistDto.PharmacyId,
                LicenseNumber = pharmacistDto.LicenseNumber,
            };

            var createdPharmacist = await _pharmacistRepository.AddAsync(pharmacist);
            if (createdPharmacist == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء الصيدلي)
                await _auditService.LogAsync(
                    action: "CreatePharmacistFailed",
                    entityName: "Pharmacist",
                    details: $"Failed to create pharmacist - Pharmacist creation failed for UserId: {createdUser.Id}");

                return null;
            }

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            await _auditService.LogAsync(
                 action: "CreatePharmacist",
                 entityName: "Pharmacist",
                 entityId: createdPharmacist.Id.ToString(),
                 details: $"Pharmacist created successfully - Name: {pharmacistDto.FullName}, Email: {pharmacistDto.Email}, LicenseNumber: {pharmacistDto.LicenseNumber}, PharmacyId: {pharmacistDto.PharmacyId}");

            return new PharmacistDto
            {
                Id = createdPharmacist.Id,
                UserId = createdPharmacist.UserId,
                PharmacyId = createdPharmacist.PharmacyId,
                LicenseNumber = createdPharmacist.LicenseNumber ?? "",
            };
        }
        catch (Exception ex)
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();

            // ← تسجيل خطأ في الإنشاء
            await _auditService.LogAsync(
                action: "CreatePharmacistError",
                entityName: "Pharmacist",
                details: $"Error creating pharmacist - Email: {pharmacistDto.Email}, Error: {ex.Message}");

            return null;
        }


    }

    public async Task<PharmacistDto> UpdateAsync(int id, UpdatePharmacistDto pharmacistDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var pharmacist = await _pharmacistRepository.GetByIdAsync(id);
            if (pharmacist == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (الصيدلي غير موجود)
                await _auditService.LogAsync(
                    action: "UpdatePharmacistFailed",
                    entityName: "Pharmacist",
                    entityId: id.ToString(),
                    details: $"Failed to update pharmacist - Pharmacist with ID {id} not found");

                return null;
            }

            var user = await _userRepository.GetByIdAsync(pharmacist.UserId);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (المستخدم غير موجود)
                await _auditService.LogAsync(
                    action: "UpdatePharmacistFailed",
                    entityName: "Pharmacist",
                    entityId: id.ToString(),
                    details: $"Failed to update pharmacist - User with ID {pharmacist.UserId} not found");

                return null;
            }

            // حفظ القيم القديمة للتسجيل
            var oldFullName = user.FullName;
            var oldEmail = user.Email;
            var oldLicenseNumber = pharmacist.LicenseNumber;
            var oldPharmacyId = pharmacist.PharmacyId;


            // تحديث بيانات المستخدم
            user.FullName = pharmacistDto.FullName;
            user.Email = pharmacistDto.Email;
            var updatedUser = await _userRepository.UpdateAsync(user);

            // تحديث بيانات المريض
            pharmacist.LicenseNumber = pharmacistDto.LicenseNumber;
            pharmacist.PharmacyId = pharmacistDto.PharmacyId;
            var updatedPatient = await _pharmacistRepository.UpdateAsync(pharmacist);

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            // ← تسجيل نجاح التحديث
            var changes = new List<string>();
            if (oldFullName != pharmacistDto.FullName) changes.Add($"Name: {oldFullName} -> {pharmacistDto.FullName}");
            if (oldEmail != pharmacistDto.Email) changes.Add($"Email: {oldEmail} -> {pharmacistDto.Email}");
            if (oldLicenseNumber != pharmacistDto.LicenseNumber) changes.Add($"LicenseNumber: {oldLicenseNumber} -> {pharmacistDto.LicenseNumber}");
            if (oldPharmacyId != pharmacistDto.PharmacyId) changes.Add($"PharmacyId: {oldPharmacyId} -> {pharmacistDto.PharmacyId}");

            await _auditService.LogAsync(
                action: "UpdatePharmacist",
                entityName: "Pharmacist",
                entityId: id.ToString(),
                details: $"Pharmacist updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

            return new PharmacistDto
            {
                Id = pharmacist.Id,
                UserId = pharmacist.UserId,
                PharmacyId = pharmacist.PharmacyId,
                LicenseNumber = pharmacist.LicenseNumber,
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            // ← تسجيل خطأ في التحديث
            await _auditService.LogAsync(
                action: "UpdatePharmacistError",
                entityName: "Pharmacist",
                entityId: id.ToString(),
                details: $"Error updating pharmacist ID {id} - Error: {ex.Message}");

            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pharmacist = await _pharmacistRepository.GetByIdAsync(id);

        if (pharmacist == null)
        {
            // ← تسجيل فشل الحذف (الصيدلي غير موجود)
            await _auditService.LogAsync(
                action: "DeletePharmacistFailed",
                entityName: "Pharmacist",
                entityId: id.ToString(),
                details: $"Failed to delete pharmacist - Pharmacist with ID {id} not found");

            return false;
        }

        var pharmacistName = pharmacist.User?.FullName ?? "Unknown";
        var result = await _pharmacistRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeletePharmacist",
                entityName: "Pharmacist",
                entityId: id.ToString(),
                details: $"Pharmacist deleted successfully - Name: {pharmacistName}");
        }

        return result;
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

    public async Task<PharmacistDto> GetPharmacistByUserIdAsync(int userId)
    {
        var pharmacist = await _pharmacistRepository.GetPharmacistByUserIdAsync(userId);
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