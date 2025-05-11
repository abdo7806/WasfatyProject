
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;
using Wasfaty.Infrastructure.Repositories;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
   // private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
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
        }; 
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto patientDto)
    {


        var patient = new Patient
        {
            UserId = patientDto.UserId,
            Gender = patientDto.Gender,
            DateOfBirth = patientDto.DateOfBirth,
            BloodType = patientDto.BloodType

        };
       var createPatient = await _patientRepository.AddAsync(patient);
        if(createPatient != null)
        {
            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                DateOfBirth = patient.DateOfBirth,
            };
        }
        return null;
    }

    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientDto patientDto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient == null)
            return null;

       patient.Gender = patientDto.Gender;
       patient.BloodType = patientDto.BloodType;
       patient.DateOfBirth = patientDto.DateOfBirth;


       

        var updatePatient = await _patientRepository.UpdateAsync(patient);

        if (updatePatient != null)
        {
            return new PatientDto
            {
                Id = updatePatient.Id,
                UserId = updatePatient.UserId,
                Gender = updatePatient.Gender,
                BloodType = updatePatient.BloodType,
                DateOfBirth = updatePatient.DateOfBirth,
            };
        }
        return null;

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
}