using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto);
        Task<PrescriptionDto> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionDto>> GetAllAsync();
        Task<PrescriptionDto> UpdateAsync(int id, CreatePrescriptionDto prescriptionDto);
        Task<bool> DeleteAsync(int id);
    }
}
