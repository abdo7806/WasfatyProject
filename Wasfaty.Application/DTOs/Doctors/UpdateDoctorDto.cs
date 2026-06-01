using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class UpdateDoctorDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int MedicalCenterId { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
    }
}
