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

    public PharmacistService(IPharmacistRepository pharmacistRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _pharmacistRepository = pharmacistRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
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
            var user = new User
            {
                FullName = pharmacistDto.FullName,
                Email = pharmacistDto.Email,
                RoleId = (int)UserRoleEnum.Pharmacist,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(pharmacistDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);
            if (createdUser == null) return null;



            var pharmacist = new Pharmacist
            {
                UserId = createdUser.Id,
                PharmacyId = pharmacistDto.PharmacyId,
                LicenseNumber = pharmacistDto.LicenseNumber,
            };

            var createPharmacist = await _pharmacistRepository.AddAsync(pharmacist);
            if (createPharmacist == null) return null;

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            return new PharmacistDto
            {
                Id = createPharmacist.Id,
                UserId = createPharmacist.UserId,
                PharmacyId = createPharmacist.PharmacyId,
                LicenseNumber = createPharmacist.LicenseNumber ?? "",
            };
        }
        catch
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();
        }

        return null;

    }

    public async Task<PharmacistDto> UpdateAsync(int id, UpdatePharmacistDto pharmacistDto)
    {

        //var existingPharmacist = await _pharmacistRepository.GetByIdAsync(id);
        //if (existingPharmacist == null) return null;

        ////  existingPharmacist.UserId = existingPharmacist.UserId;
        //existingPharmacist.LicenseNumber = pharmacistDto.LicenseNumber;
        //existingPharmacist.PharmacyId = pharmacistDto.PharmacyId;
        //Pharmacist Pharmacist = await _pharmacistRepository.UpdateAsync(existingPharmacist);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var pharmacist = await _pharmacistRepository.GetByIdAsync(id);
            if (pharmacist == null)
                return null;

            var user = await _userRepository.GetByIdAsync(pharmacist.UserId);
            if (user == null)
                return null;

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

            return new PharmacistDto
            {
                Id = pharmacist.Id,
                UserId = pharmacist.UserId,
                PharmacyId = pharmacist.PharmacyId,
                LicenseNumber = pharmacist.LicenseNumber,
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
        }
        return null;
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