using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Doctors
{
    public class CreateDoctorDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int MedicalCenterId { get; set; }
        [Required]
        public string? Specialization { get; set; }
        [Required]
        public string? LicenseNumber { get; set; }
    }
}
