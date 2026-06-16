using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;
        public UserService(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
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
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = (UserRoleEnum)user.RoleId,
                CreatedAt = user.CreatedAt
            });
        }


        public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
        {
            // التحقق من وجود البريد الإلكتروني مسبقاً
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                // ← تسجيل فشل الإنشاء (بريد موجود)
                await _auditService.LogAsync(
                    action: "CreateUserFailed",
                    entityName: "User",
                    details: $"Failed to create user - Email already exists: {userDto.Email}");

                return null;
            }

            var user = new User
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                RoleId = (int)userDto.Role,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            var createdUser = await _userRepository.AddAsync(user);

            if (createdUser != null)
            {
                // ← تسجيل نجاح الإنشاء
                await _auditService.LogAsync(
                    action: "CreateUser",
                    entityName: "User",
                    entityId: createdUser.Id.ToString(),
                    details: $"User created successfully - Name: {userDto.FullName}, Email: {userDto.Email}, Role: {userDto.Role}");

                return new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName
                };
            }

            // ← تسجيل فشل الإنشاء (خطأ غير متوقع)
            await _auditService.LogAsync(
                action: "CreateUserFailed",
                entityName: "User",
                details: $"Failed to create user - Unknown error for Email: {userDto.Email}");

            return null;
        }
        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                // ← تسجيل فشل التحديث (المستخدم غير موجود)
                await _auditService.LogAsync(
                    action: "UpdateUserFailed",
                    entityName: "User",
                    entityId: id.ToString(),
                    details: $"Failed to update user - User with ID {id} not found");

                return null;
            }

            // حفظ القيم القديمة للتسجيل
            var oldFullName = user.FullName;
            var oldEmail = user.Email;
            var oldRoleId = user.RoleId;
            var oldRole = (UserRoleEnum)oldRoleId;

            // التحقق من عدم تكرار البريد الإلكتروني (إذا تم تغييره)
            if (oldEmail != userDto.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    // ← تسجيل فشل التحديث (بريد مكرر)
                    await _auditService.LogAsync(
                        action: "UpdateUserFailed",
                        entityName: "User",
                        entityId: id.ToString(),
                        details: $"Failed to update user - Email already exists: {userDto.Email}");

                    return null;
                }
            }

            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.RoleId = (int)userDto.Role;

            var updatedUser = await _userRepository.UpdateAsync(user);

            // ← تسجيل نجاح التحديث
            var changes = new List<string>();
            if (oldFullName != userDto.FullName) changes.Add($"Name: {oldFullName} -> {userDto.FullName}");
            if (oldEmail != userDto.Email) changes.Add($"Email: {oldEmail} -> {userDto.Email}");
            if (oldRole != userDto.Role) changes.Add($"Role: {oldRole} -> {userDto.Role}");

            await _auditService.LogAsync(
                action: "UpdateUser",
                entityName: "User",
                entityId: id.ToString(),
                details: $"User updated. Changes: {(changes.Any() ? string.Join(", ", changes) : "No changes")}");

            return new UserDto
            {
                Id = updatedUser.Id,
                Email = updatedUser.Email,
                FullName = updatedUser.FullName,
                Role = (UserRoleEnum)updatedUser.RoleId
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                // ← تسجيل فشل الحذف (المستخدم غير موجود)
                await _auditService.LogAsync(
                    action: "DeleteUserFailed",
                    entityName: "User",
                    entityId: id.ToString(),
                    details: $"Failed to delete user - User with ID {id} not found");

                return false;
            }

            var userName = user.FullName;
            var userEmail = user.Email;

            // التحقق من أن المستخدم ليس مرتبطاً ببيانات أخرى (اختياري)
            // يمكنك إضافة منطق للتحقق من وجود Patient, Doctor, Pharmacist مرتبط بهذا المستخدم

            var result = await _userRepository.DeleteAsync(id);

            if (result)
            {
                // ← تسجيل نجاح الحذف
                await _auditService.LogAsync(
                    action: "DeleteUser",
                    entityName: "User",
                    entityId: id.ToString(),
                    details: $"User deleted successfully - Name: {userName}, Email: {userEmail}");
            }

            return result;
        }
    }
}
