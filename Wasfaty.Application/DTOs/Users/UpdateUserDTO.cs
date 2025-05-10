using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Users
{
    public class UpdateUserDto
    {
      //  public int Id { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public UserRoleEnum? Role { get; set; } // دور المستخدم قد لا يتم تغييره، لذا يمكن أن يكون اختياريًا

        // public string? Password { get; set; }
    }
}
