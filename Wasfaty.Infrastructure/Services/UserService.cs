using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = (UserRoleEnum)user.RoleId,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
        {


            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                RoleId = (int)userDto.Role,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password) // يجب إضافة منطق لتشفير كلمة المرور
            };


            var createdUser = await _userRepository.AddAsync(user);

            if (createdUser != null)
            {
                return new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName
                };
            }
            return null;
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) 
                return null;

            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.RoleId = (int)userDto.Role;

          
            var createdUser = await _userRepository.UpdateAsync(user); ;

            return new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FullName = createdUser.FullName
            };

        }

        public async Task<bool> DeleteUserAsync(int id)
        {
           return await _userRepository.DeleteAsync(id);
        }
    }
}
