using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces.IRepositories;
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User?> GetByIdWithRoleAsync(int id);
    Task<User?> GetByEmailAsync(string email);

    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);


}