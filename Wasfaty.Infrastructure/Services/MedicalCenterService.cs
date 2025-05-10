using System.Net;
using System.Numerics;
using Wasfaty.Application.DTOs.MedicalCenters;
using Wasfaty.Application.Interfaces;

public class MedicalCenterService : IMedicalCenterService
{
    private readonly IMedicalCenterRepository _medicalCenterRepository;

    public MedicalCenterService(IMedicalCenterRepository medicalCenterRepository)
    {
        _medicalCenterRepository = medicalCenterRepository;
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
        var medicalCenter = new MedicalCenter
        {
           
            Name = medicalCenterDto.Name,
            Address = medicalCenterDto.Address,
            Phone = medicalCenterDto.Phone,
        };


        MedicalCenter createMedicalCenter = await _medicalCenterRepository.AddAsync(medicalCenter);

        if(createMedicalCenter != null)
        {
            return new MedicalCenterDto
            {
                Id = createMedicalCenter.Id,
                Name = createMedicalCenter.Name,
                Address = createMedicalCenter.Address,
                Phone = createMedicalCenter.Phone,
            };
        }


        return null;
    }

    public async Task<MedicalCenterDto> UpdateAsync(int id, UpdateMedicalCenterDto medicalCenterDto)
    {

        // احصل على المركز الطبي الحالي
        MedicalCenter existingMedicalCenter = await _medicalCenterRepository.GetByIdAsync(id);

        if (existingMedicalCenter == null)
            return null;

        // تحديث الخصائص مباشرة
        existingMedicalCenter.Name = medicalCenterDto.Name;
        existingMedicalCenter.Address = medicalCenterDto.Address;
        existingMedicalCenter.Phone = medicalCenterDto.Phone;

        // تحديث الكائن في قاعدة البيانات
        await _medicalCenterRepository.UpdateAsync(existingMedicalCenter);

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
        return await _medicalCenterRepository.DeleteAsync(id);
    }
}