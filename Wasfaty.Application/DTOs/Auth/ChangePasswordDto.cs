using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; } // معرف المستخدم

        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } // كلمة المرور الحالية

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, ErrorMessage = "New password must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } // كلمة المرور الجديدة
    }
}
