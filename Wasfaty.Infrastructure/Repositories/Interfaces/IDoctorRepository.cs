using Wasfaty.Application.DTOs.Doctors;
//using Wasfaty.Domain.Entities;

namespace Wasfaty.Application.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int id);
       // Task SaveChangesAsync();
    }
}
