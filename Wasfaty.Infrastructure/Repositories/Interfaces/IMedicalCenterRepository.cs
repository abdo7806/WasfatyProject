using Wasfaty.Application.DTOs.MedicalCenters;

public interface IMedicalCenterRepository
{
    Task<IEnumerable<MedicalCenter>> GetAllAsync();
    Task<MedicalCenter> GetByIdAsync(int id);
    Task<MedicalCenter> AddAsync(MedicalCenter medicalCenter);
    Task<MedicalCenter> UpdateAsync(MedicalCenter medicalCenter);
    Task<bool> DeleteAsync(int id);
}