using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces
{
    public interface IService<T>
    {
      
        Task<T> CreateUserAsync(T request);

    
        Task<T> UpdateUserAsync(int id, T request);

        Task<bool> DeleteUserAsync(int id);

  
        Task<IEnumerable<T>> GetAllUsersAsync();

        Task<T> GetUserByIdAsync(int id);
    }
}
