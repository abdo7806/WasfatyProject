using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces
{
    public interface IAuthService
    {

        /// <summary>
        /// تسجل مستخدم جديد
        /// </summary>
        /// <param name="request">بيانات المستخدم</param>
        /// <returns>نتيجة عملية التسجيل (مثلاً توكن أو رسالة نجاح)</returns>
        Task<UserDto> RegisterAsync(RegisterUserDto request);

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        /// <param name="request">بيانات تسجيل الدخول</param>
        /// <returns>توكن JWT إذا نجح الدخول</returns>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        /* Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
         Task<LoginRequestDto> LoginAsync(LoginDTO loginDto);
         Task<bool> UserExistsAsync(string email);*/
    



    }
}
