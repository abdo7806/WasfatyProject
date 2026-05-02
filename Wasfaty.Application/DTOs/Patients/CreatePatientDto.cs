using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Patients
{
    public class CreatePatientDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        //public UserRoleEnum Role { get; set; }

        [Required]
        public string Password { get; set; }

        //[Required]
        //public int UserId { get; set; }

        // [Required]
        // [MaxLength(50)]
        // public string MedicalRecordNumber { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? BloodType { get; set; }
    }
}
