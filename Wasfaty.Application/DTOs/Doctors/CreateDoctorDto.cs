using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class CreateDoctorDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int MedicalCenterId { get; set; }
        [Required]
        public string? Specialization { get; set; }
        [Required]
        public string? LicenseNumber { get; set; }
    }
}
