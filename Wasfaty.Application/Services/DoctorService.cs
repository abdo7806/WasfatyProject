using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.MedicalCenters;
using System.Numerics;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.Pharmacies;
using Wasfaty.Application.Interfaces.IServices;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.DTOs.Patients;
namespace Wasfaty.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;


    public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
    {
        var Doctor = await _doctorRepository.GetAllAsync();

        return Doctor.Select(doctor => new DoctorDto
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            MedicalCenterId = doctor.MedicalCenterId,
            Specialization = doctor.Specialization,
            LicenseNumber = doctor.LicenseNumber,
            User = new UserDto
            {
                Id = doctor.User.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                Role = (UserRoleEnum)doctor.User.RoleId,
                CreatedAt = doctor.User.CreatedAt,

            },
            MedicalCenter = new MedicalCenterDto
            {
                Id = doctor.MedicalCenter.Id,
                Name = doctor.MedicalCenter.Name,
                Address = doctor.MedicalCenter.Address,
                Phone = doctor.MedicalCenter.Phone,
            }

        });
        //  return await _doctorRepository.GetAllAsync();
    }

    public async Task<DoctorDto> GetDoctorByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor != null)
        {
            return new  DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                MedicalCenterId = doctor.MedicalCenterId,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = doctor.User.Id,
                    FullName = doctor.User.FullName,
                    Email = doctor.User.Email,
                    Role = (UserRoleEnum)doctor.User.RoleId,
                    CreatedAt = doctor.User.CreatedAt,

                },
                MedicalCenter = new MedicalCenterDto
                {
                    Id = doctor.MedicalCenter.Id,
                    Name = doctor.MedicalCenter.Name,
                    Address = doctor.MedicalCenter.Address,
                    Phone = doctor.MedicalCenter.Phone,
                }

            };
        }
        return null;
    }

    public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctorDto)
    {
        // بدء Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                FullName = doctorDto.FullName,
                Email = doctorDto.Email,
                RoleId = (int)UserRoleEnum.Doctor,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(doctorDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);
            if (createdUser == null) return null;

  

            var doctor = new Doctor
            {
                UserId = createdUser.Id,
                MedicalCenterId = doctorDto.MedicalCenterId,
                Specialization = doctorDto.Specialization,
                LicenseNumber = doctorDto.LicenseNumber,
            };

            var createDoctor = await _doctorRepository.AddAsync(doctor);
            if (createDoctor == null) return null;

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            return new DoctorDto
            {
                Id = createDoctor.Id,
                UserId = createDoctor.UserId,
                MedicalCenterId = createDoctor.MedicalCenterId,
                Specialization = createDoctor.Specialization,
                LicenseNumber = createDoctor.LicenseNumber,
            };
        }
        catch
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();
        }

        return null;
       
    }

    public async Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto doctorDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return null;

            var user = await _userRepository.GetByIdAsync(doctor.UserId);
            if (user == null)
                return null;

            // تحديث بيانات المستخدم
            user.FullName = doctorDto.FullName;
            user.Email = doctorDto.Email;
            var updatedUser = await _userRepository.UpdateAsync(user);


            var updatedDoctor = await _doctorRepository.UpdateAsync(doctor);

            updatedDoctor.MedicalCenterId = doctorDto.MedicalCenterId;
            updatedDoctor.Specialization = doctorDto.Specialization;
            updatedDoctor.LicenseNumber = doctorDto.LicenseNumber;

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            return new DoctorDto
            {
                Id = updatedDoctor.Id,
                UserId = updatedDoctor.UserId,
                MedicalCenterId = updatedDoctor.MedicalCenterId,
                Specialization = updatedDoctor.Specialization,
                LicenseNumber = updatedDoctor.LicenseNumber,
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
        }
        return null;
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
       return await _doctorRepository.DeleteAsync(id);
    }

    public async Task<DoctorDto> GetDoctorByUserIdAsync(int userId)
    {

        var doctor = await _doctorRepository.GetByUserIdAsync(userId);
        if (doctor != null)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                MedicalCenterId = doctor.MedicalCenterId,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                User = new UserDto
                {
                    Id = doctor.User.Id,
                    FullName = doctor.User.FullName,
                    Email = doctor.User.Email,
                    Role = (UserRoleEnum)doctor.User.RoleId,
                    CreatedAt = doctor.User.CreatedAt,

                },
                MedicalCenter = new MedicalCenterDto
                {
                    Id = doctor.MedicalCenter.Id,
                    Name = doctor.MedicalCenter.Name,
                    Address = doctor.MedicalCenter.Address,
                    Phone = doctor.MedicalCenter.Phone,
                }

            };
        }
        return null;
    }

    public Task<DoctorDashboardDto> GetDashboardAsync(int doctorId)
    {
        return _doctorRepository.GetDashboardAsync(doctorId);
    }
}