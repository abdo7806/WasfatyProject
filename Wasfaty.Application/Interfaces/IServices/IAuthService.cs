using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.Interfaces.IServices
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
        //Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        //Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string deviceInfo = null, string ipAddress = null)

        Task<(LoginResponseDto Result, string RefreshToken)> LoginAsync(
            LoginRequestDto request,
            string deviceInfo = null,
            string ipAddress = null);


        /* Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
         Task<LoginRequestDto> LoginAsync(LoginDTO loginDto);
         Task<bool> UserExistsAsync(string email);*/

        /// <summary>
        /// تغير كلمة المرور 
        /// </summary>
        /// <param name="request">بيانات تسجيل الدخول</param>
        /// <returns>توكن JWT إذا نجح الدخول</returns>
        Task<bool> ChangeUserPassword(int userId, string currentPassword, string newPassword);



        //Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

        //  تعديل الدالة لترجع الـ Refresh Token الجديد
        Task<(RefreshTokenResponseDto Result, string NewRefreshToken)> RefreshTokenAsync(string refreshToken);

        Task<bool> LogoutAsync(int userId, string refreshToken);
        Task<bool> RevokeAllUserTokensAsync(int userId);
    }
}
