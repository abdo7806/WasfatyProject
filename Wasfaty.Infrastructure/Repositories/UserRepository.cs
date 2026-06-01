using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IRepositories;

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

        public async Task<User?> GetByIdWithRoleAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
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
            try
            {
                // نتحقق من ان مافي مستخدم بنفس البريد الاكتروني
                if (!_context.Users.Any(u => u.Email == user.Email))
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<User> UpdateAsync(User user)
        {
            try
            {
                if (!_context.Users.Any(u => u.Email == user.Email && u.Id != user.Id))
                {

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return user;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }

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
