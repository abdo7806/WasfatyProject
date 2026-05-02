
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

    // private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
       // var patients = await _patientRepository.GetAllAsync();
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

    //public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto)
    //{

    //    var user = new User
    //    {
    //        FullName = patientDto.FullName,
    //        Email = patientDto.Email,
    //        RoleId = (int)patientDto.Role,
    //        CreatedAt = DateTime.UtcNow,
    //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(patientDto.Password) // يجب إضافة منطق لتشفير كلمة المرور
    //    };


    //    var createdUser = await _userRepository.AddAsync(user);

    //    if (createdUser == null) return null;

    //    var patient = new Patient
    //    {
    //        UserId = createdUser.Id,
    //        Gender = patientDto.Gender,
    //        DateOfBirth = patientDto.DateOfBirth,
    //        BloodType = patientDto.BloodType

    //    };
    //   var createPatient = await _patientRepository.AddAsync(patient);
    //    if(createPatient != null)
    //    {
    //        return new PatientDto
    //        {
    //            Id = patient.Id,
    //            UserId = patient.UserId,
    //            Gender = patient.Gender,
    //            BloodType = patient.BloodType,
    //            DateOfBirth = patient.DateOfBirth,
    //        };
    //    }
    //    return null;
    //}

    public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto)
    {
        // بدء Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = new User
            {
                FullName = patientDto.FullName,
                Email = patientDto.Email,
                RoleId = (int)UserRoleEnum.Patient,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(patientDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);
            if (createdUser == null) return null;

            var patient = new Patient
            {
                UserId = createdUser.Id,
                Gender = patientDto.Gender,
                DateOfBirth = patientDto.DateOfBirth,
                BloodType = patientDto.BloodType
            };

            var createdPatient = await _patientRepository.AddAsync(patient);
            if (createdPatient == null) return null;

            // كل شيء نجح → نثبت العملية
            await _unitOfWork.CommitAsync();

            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                DateOfBirth = patient.DateOfBirth,
            };
        }
        catch
        {
            // أي خطأ → نلغي كل شيء
            await _unitOfWork.RollbackAsync();
            throw;
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
                return null;

            var user = await _userRepository.GetByIdAsync(patient.UserId);
            if (user == null)
                return null;

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

            return new PatientDto
            {
                Id = updatedPatient.Id,
                UserId = updatedPatient.UserId,
                Gender = updatedPatient.Gender,
                BloodType = updatedPatient.BloodType,
                DateOfBirth = updatedPatient.DateOfBirth,
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    public async Task<bool> DeleteAsync(int id)
    {
       return await _patientRepository.DeleteAsync(id);
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