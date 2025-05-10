using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Users
{
    public class CreateUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public UserRoleEnum Role { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
