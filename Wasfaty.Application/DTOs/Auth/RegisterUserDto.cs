using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Auth
{
    public class RegisterUserDto// انشاء حساب
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; }
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 أحرف")]
        public string Password { get; set; }

        [Required(ErrorMessage = "نوع المستخدم مطلوب")]
        [EnumDataType(typeof(UserRoleEnum), ErrorMessage = "نوع المستخدم غير صالح")]
        public UserRoleEnum Role { get; set; }

       /* public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } // حسب الجدول الموجود عندك*/
    }
}
