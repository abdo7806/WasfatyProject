using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces.IRepositories;
public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}