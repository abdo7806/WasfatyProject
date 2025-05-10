using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// إنشاء مستخدم جديد
        /// </summary>
        /// <param name="request">بيانات المستخدم الجديد</param>
        /// <returns>معلومات المستخدم الذي تم إنشاؤه</returns>
        Task<UserDto> CreateUserAsync(CreateUserDto request);

        /// <summary>
        /// تحديث بيانات المستخدم
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <param name="request">بيانات المستخدم المعدلة</param>
        /// <returns>معلومات المستخدم المحدث</returns>
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto request);

        /// <summary>
        /// حذف مستخدم
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <returns>مؤشر على نجاح عملية الحذف</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// استرجاع جميع المستخدمين
        /// </summary>
        /// <returns>قائمة بالمستخدمين</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// استرجاع مستخدم حسب معرفه
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <returns>بيانات المستخدم</returns>
        Task<UserDto> GetUserByIdAsync(int id);
    }
}
