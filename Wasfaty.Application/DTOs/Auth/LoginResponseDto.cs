using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Auth
{
    public class LoginResponseDto// النتيجه من تسجيل الدخول
    {
        public string AccessToken { get; set; }  // لا يوجد RefreshToken هنا
        public UserDto User { get; set; }
        public int ExpiresIn { get; set; }  // 7200 ثانية
    }
}
