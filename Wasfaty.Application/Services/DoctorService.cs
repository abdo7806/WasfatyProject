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
    private readonly IAuditService _auditService;


    public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, IAuditService auditService)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _auditService = auditService;
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




            // التحقق من وجود البريد الإلكتروني مسبقاً
            var existingUser = await _userRepository.GetByEmailAsync(doctorDto.Email);
            if (existingUser != null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (بريد موجود)
                await _auditService.LogAsync(
                    action: "CreateDoctorFailed",
                    entityName: "Doctor",
                    details: $"Failed to create doctor - Email already exists: {doctorDto.Email}");

                return null;
            }

            var user = new User
            {
                FullName = doctorDto.FullName,
                Email = doctorDto.Email,
                RoleId = (int)UserRoleEnum.Doctor,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(doctorDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);

            if (createdUser == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء المستخدم)
                await _auditService.LogAsync(
                    action: "CreateDoctorFailed",
                    entityName: "Doctor",
                    details: $"Failed to create doctor - User creation failed for email: {doctorDto.Email}");

                return null;
            }


            var doctor = new Doctor
            {
                UserId = createdUser.Id,
                MedicalCenterId = doctorDto.MedicalCenterId,
                Specialization = doctorDto.Specialization,
                LicenseNumber = doctorDto.LicenseNumber,
            };

            var createdDoctor = await _doctorRepository.AddAsync(doctor);

            if (createdDoctor == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء الدكتور)
                await _auditService.LogAsync(
                    action: "CreateDoctorFailed",
                    entityName: "Doctor",
                    details: $"Failed to create doctor - Doctor creation failed for UserId: {createdUser.Id}");

                return null;
            }

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();


            // ← تسجيل نجاح الإنشاء
            await _auditService.LogAsync(
                action: "CreateDoctor",
                entityName: "Doctor",
                entityId: createdDoctor.Id.ToString(),
                details: $"Doctor created successfully - Name: {doctorDto.FullName}, Email: {doctorDto.Email}, Specialization: {doctorDto.Specialization}");


            return new DoctorDto
            {
                Id = createdDoctor.Id,
                UserId = createdDoctor.UserId,
                MedicalCenterId = createdDoctor.MedicalCenterId,
                Specialization = createdDoctor.Specialization,
                LicenseNumber = createdDoctor.LicenseNumber,
            };
        }
        catch (Exception ex)
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();
            // ← تسجيل خطأ في الإنشاء
            await _auditService.LogAsync(
                action: "CreateDoctorError",
                entityName: "Doctor",
                details: $"Error creating doctor - Email: {doctorDto.Email}, Error: {ex.Message}");

            return null;
        }

    }

    public async Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto doctorDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {

            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (الدكتور غير موجود)
                await _auditService.LogAsync(
                    action: "UpdateDoctorFailed",
                    entityName: "Doctor",
                    entityId: id.ToString(),
                    details: $"Failed to update doctor - Doctor with ID {id} not found");

                return null;
            }

            var user = await _userRepository.GetByIdAsync(doctor.UserId);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (المستخدم غير موجود)
                await _auditService.LogAsync(
                    action: "UpdateDoctorFailed",
                    entityName: "Doctor",
                    entityId: id.ToString(),
                    details: $"Failed to update doctor - User with ID {doctor.UserId} not found");

                return null;
            }

            // حفظ القيم القديمة للتسجيل
            var oldFullName = user.FullName;
            var oldEmail = user.Email;
            var oldSpecialization = doctor.Specialization;
            var oldMedicalCenterId = doctor.MedicalCenterId;
            var oldLicenseNumber = doctor.LicenseNumber;


            // تحديث بيانات المستخدم
            user.FullName = doctorDto.FullName;
            user.Email = doctorDto.Email;
            var updatedUser = await _userRepository.UpdateAsync(user);


            doctor.MedicalCenterId = doctorDto.MedicalCenterId;
            doctor.Specialization = doctorDto.Specialization;
            doctor.LicenseNumber = doctorDto.LicenseNumber;

            var updatedDoctor = await _doctorRepository.UpdateAsync(doctor);

            updatedDoctor.MedicalCenterId = doctorDto.MedicalCenterId;
            updatedDoctor.Specialization = doctorDto.Specialization;
            updatedDoctor.LicenseNumber = doctorDto.LicenseNumber;

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();


            var changes = new List<string>();
            if (oldFullName != doctorDto.FullName) changes.Add($"Name: {oldFullName} -> {doctorDto.FullName}");
            if (oldEmail != doctorDto.Email) changes.Add($"Email: {oldEmail} -> {doctorDto.Email}");
            if (oldSpecialization != doctorDto.Specialization) changes.Add($"Specialization: {oldSpecialization} -> {doctorDto.Specialization}");
            if (oldMedicalCenterId != doctorDto.MedicalCenterId) changes.Add($"MedicalCenterId: {oldMedicalCenterId} -> {doctorDto.MedicalCenterId}");
            if (oldLicenseNumber != doctorDto.LicenseNumber) changes.Add($"LicenseNumber: {oldLicenseNumber} -> {doctorDto.LicenseNumber}");

            await _auditService.LogAsync(
                action: "UpdateDoctor",
                entityName: "Doctor",
                entityId: id.ToString(),
                details: $"Doctor updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

            return new DoctorDto
            {
                Id = updatedDoctor.Id,
                UserId = updatedDoctor.UserId,
                MedicalCenterId = updatedDoctor.MedicalCenterId,
                Specialization = updatedDoctor.Specialization,
                LicenseNumber = updatedDoctor.LicenseNumber,
            };
        }
        catch(Exception ex) 
        {
            await _unitOfWork.RollbackAsync();

            // ← تسجيل خطأ في التحديث
            await _auditService.LogAsync(
                action: "UpdateDoctorError",
                entityName: "Doctor",
                entityId: id.ToString(),
                details: $"Error updating doctor ID {id} - Error: {ex.Message}");

            return null;
        }
    }
    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);

        if (doctor == null)
        {
            // ← تسجيل فشل الحذف (الدكتور غير موجود)
            await _auditService.LogAsync(
                action: "DeleteDoctorFailed",
                entityName: "Doctor",
                entityId: id.ToString(),
                details: $"Failed to delete doctor - Doctor with ID {id} not found");

            return false;
        }

        var doctorName = doctor.User?.FullName ?? "Unknown";
        var result = await _doctorRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeleteDoctor",
                entityName: "Doctor",
                entityId: id.ToString(),
                details: $"Doctor deleted successfully - Name: {doctorName}");
        }

        return result;
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