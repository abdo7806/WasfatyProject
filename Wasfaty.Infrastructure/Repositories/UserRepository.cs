using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = (UserRoleEnum)user.RoleId,
                    CreatedAt = user.CreatedAt
                }).ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            int x = _context.Users.Where(u=> u.Email == user.Email).Count();
            if (x == 0)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            return null;

        }

        public async Task<User> UpdateAsync(User user)
        {
         
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                try
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;

                }
                catch (Exception ex) 
                {
                    return false;
                }

            }
            return false;   
        }
    }
}
