using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Auth
{
    public class LoginResponseDto// النتيجه من تسجيل الدخول
    {
        public string Token { get; set; } = string.Empty;// تحديد صلاحيات المستخدم

        public UserDto User { get; set; }
    }
}
