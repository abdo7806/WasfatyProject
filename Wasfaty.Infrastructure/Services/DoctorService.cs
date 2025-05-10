using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.Interfaces.Repositories;
using Wasfaty.Application.Interfaces;
using Wasfaty.Application.DTOs.MedicalCenters;
using System.Numerics;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.DTOs.Pharmacies;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
   

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
        
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
        var doctor = new Doctor
        {
            UserId = doctorDto.UserId,
            MedicalCenterId = doctorDto.MedicalCenterId,
            Specialization = doctorDto.Specialization,
            LicenseNumber = doctorDto.LicenseNumber,
        };

        Doctor createDoctor = await _doctorRepository.AddAsync(doctor);

        if (createDoctor != null)
        {
            return new DoctorDto
            {
                Id = createDoctor.Id,
                UserId = createDoctor.UserId,
                MedicalCenterId = createDoctor.MedicalCenterId,
                Specialization = createDoctor.Specialization,
                LicenseNumber = createDoctor.LicenseNumber,
            };
        }


        return null;
       
    }

    public async Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto doctorDto)
    {
        // احصل على المركز الطبي الحالي
        Doctor existingDoctor = await _doctorRepository.GetByIdAsync(id);

        if (existingDoctor == null)
            return null;

     //   existingDoctor.UserId = doctorDto.UserId;
        existingDoctor.MedicalCenterId = doctorDto.MedicalCenterId;
        existingDoctor.Specialization = doctorDto.Specialization;
        existingDoctor.LicenseNumber = doctorDto.LicenseNumber;

        await _doctorRepository.UpdateAsync(existingDoctor);

        return new DoctorDto
        {
            Id = existingDoctor.Id,
            UserId = existingDoctor.UserId,
            MedicalCenterId = existingDoctor.MedicalCenterId,
            Specialization = existingDoctor.Specialization,
            LicenseNumber = existingDoctor.LicenseNumber,
        };
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
       return await _doctorRepository.DeleteAsync(id);
    }
}