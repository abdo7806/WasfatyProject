
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;


namespace Wasfaty.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    // private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _patientRepository = patientRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        
        return await _patientRepository.GetAllAsync();
    }

    public async Task<PatientDto> GetByIdAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
        {
            return null;
        }

        
        return new PatientDto
        {
            Id = patient.Id,
            UserId = patient.UserId,
            Gender = patient.Gender,
            BloodType = patient.BloodType,
            DateOfBirth = patient.DateOfBirth,
            User = new UserDto
            {
                Id = patient.User.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                Role = (UserRoleEnum)patient.User.RoleId,
                CreatedAt = patient.User.CreatedAt,
            },

            Prescriptions = patient.Prescriptions.Select(p => new PrescriptionDto
            {
                Id = p.Id,
                DoctorId = p.DoctorId,
                PatientId = p.PatientId,
                IssuedDate = p.IssuedDate,
                IsDispensed = p.IsDispensed,
            })

        }; 
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto)
    {
        // بدء Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // التحقق من وجود البريد الإلكتروني مسبقاً
            var existingUser = await _userRepository.GetByEmailAsync(patientDto.Email);
            if (existingUser != null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (بريد موجود)
                await _auditService.LogAsync(
                    action: "CreatePatientFailed",
                    entityName: "Patient",
                    details: $"Failed to create patient - Email already exists: {patientDto.Email}");

                return null;
            }

            var user = new User
            {
                FullName = patientDto.FullName,
                Email = patientDto.Email,
                RoleId = (int)UserRoleEnum.Patient,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(patientDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);
            if (createdUser == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء المستخدم)
                await _auditService.LogAsync(
                    action: "CreatePatientFailed",
                    entityName: "Patient",
                    details: $"Failed to create patient - User creation failed for email: {patientDto.Email}");

                return null;
            }

            var patient = new Patient
            {
                UserId = createdUser.Id,
                Gender = patientDto.Gender,
                DateOfBirth = patientDto.DateOfBirth,
                BloodType = patientDto.BloodType
            };

            var createdPatient = await _patientRepository.AddAsync(patient);
            if (createdPatient == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل الإنشاء (فشل إنشاء المريض)
                await _auditService.LogAsync(
                    action: "CreatePatientFailed",
                    entityName: "Patient",
                    details: $"Failed to create patient - Patient creation failed for UserId: {createdUser.Id}");

                return null;
            }
            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            // ← تسجيل نجاح الإنشاء
            await _auditService.LogAsync(
                action: "CreatePatient",
                entityName: "Patient",
                entityId: createdPatient.Id.ToString(),
                details: $"Patient created successfully - Name: {patientDto.FullName}, Email: {patientDto.Email}, Gender: {patientDto.Gender}");

            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                DateOfBirth = patient.DateOfBirth,
            };
        }
        catch (Exception ex)
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();

            // ← تسجيل خطأ في الإنشاء
            await _auditService.LogAsync(
                action: "CreatePatientError",
                entityName: "Patient",
                details: $"Error creating patient - Email: {patientDto.Email}, Error: {ex.Message}");

            return null;

        }
    }
    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientDto patientDto)
    {
        // بدء Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (المريض غير موجود)
                await _auditService.LogAsync(
                    action: "UpdatePatientFailed",
                    entityName: "Patient",
                    entityId: id.ToString(),
                    details: $"Failed to update patient - Patient with ID {id} not found");

                return null;
            }

            var user = await _userRepository.GetByIdAsync(patient.UserId);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync();

                // ← تسجيل فشل التحديث (المستخدم غير موجود)
                await _auditService.LogAsync(
                    action: "UpdatePatientFailed",
                    entityName: "Patient",
                    entityId: id.ToString(),
                    details: $"Failed to update patient - User with ID {patient.UserId} not found");

                return null;
            }

            // حفظ القيم القديمة للتسجيل
            var oldFullName = user.FullName;
            var oldEmail = user.Email;
            var oldGender = patient.Gender;
            var oldBloodType = patient.BloodType;
            var oldDateOfBirth = patient.DateOfBirth;


            // تحديث بيانات المستخدم
            user.FullName = patientDto.FullName;
            user.Email = patientDto.Email;
            var updatedUser = await _userRepository.UpdateAsync(user);

            // تحديث بيانات المريض
            patient.Gender = patientDto.Gender;
            patient.BloodType = patientDto.BloodType;
            patient.DateOfBirth = patientDto.DateOfBirth;
            var updatedPatient = await _patientRepository.UpdateAsync(patient);

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();


            var changes = new List<string>();
            if (oldFullName != patientDto.FullName) changes.Add($"Name: {oldFullName} -> {patientDto.FullName}");
            if (oldEmail != patientDto.Email) changes.Add($"Email: {oldEmail} -> {patientDto.Email}");
            if (oldGender != patientDto.Gender) changes.Add($"Gender: {oldGender} -> {patientDto.Gender}");
            if (oldBloodType != patientDto.BloodType) changes.Add($"BloodType: {oldBloodType} -> {patientDto.BloodType}");
            if (oldDateOfBirth != patientDto.DateOfBirth) changes.Add($"DateOfBirth: {oldDateOfBirth} -> {patientDto.DateOfBirth}");

            await _auditService.LogAsync(
                action: "UpdatePatient",
                entityName: "Patient",
                entityId: id.ToString(),
                details: $"Patient updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

            return new PatientDto
            {
                Id = updatedPatient.Id,
                UserId = updatedPatient.UserId,
                Gender = updatedPatient.Gender,
                BloodType = updatedPatient.BloodType,
                DateOfBirth = updatedPatient.DateOfBirth,
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            await _auditService.LogAsync(
                 action: "UpdatePatientError",
                 entityName: "Patient",
                 entityId: id.ToString(),
                 details: $"Error updating patient ID {id} - Error: {ex.Message}");

            return null;
        }

    }
    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
        {
            // ← تسجيل فشل الحذف (المريض غير موجود)
            await _auditService.LogAsync(
                action: "DeletePatientFailed",
                entityName: "Patient",
                entityId: id.ToString(),
                details: $"Failed to delete patient - Patient with ID {id} not found");

            return false;
        }

        var patientName = patient.User?.FullName ?? "Unknown";
        var result = await _patientRepository.DeleteAsync(id);

        if (result)
        {
            // ← تسجيل نجاح الحذف
            await _auditService.LogAsync(
                action: "DeletePatient",
                entityName: "Patient",
                entityId: id.ToString(),
                details: $"Patient deleted successfully - Name: {patientName}");
        }

        return result;
    }

    public async Task<List<PatientDto>> SearchPatients(string term)
    {
        List<Patient> Patients = await _patientRepository.SearchPatients(term);

        return Patients.Select(patient => new PatientDto
        {
            Id = patient.Id,
            UserId = patient.UserId,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            BloodType = patient.BloodType,
            User = new UserDto
            {
                Id = patient.User.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                Role = (UserRoleEnum)patient.User.RoleId,
                CreatedAt = patient.User.CreatedAt,
            },

        }).ToList(); // يتضمن الوصفات
    }

    public async Task<PatientDto> GetPatientByUserIdAsync(int userId)
    {
        var patient = await _patientRepository.GetPatientByUserIdAsync(userId);

        if (patient == null)
        {
            return null;
        }

        

        return new PatientDto
        {
            Id = patient.Id,
            UserId = patient.UserId,
            Gender = patient.Gender,
            BloodType = patient.BloodType,
            DateOfBirth = patient.DateOfBirth,
            User = new UserDto
            {
                Id = patient.User.Id,
                FullName = patient.User.FullName,
                Email = patient.User.Email,
                Role = (UserRoleEnum)patient.User.RoleId,
                CreatedAt = patient.User.CreatedAt,
            },
        };
    }

    public Task<PatientDashboardDto> GetDashboardDataAsync(int patientId)
    {
        return _patientRepository.GetDashboardDataAsync(patientId);
    }
}