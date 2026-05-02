using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Patients
{
    public class UpdatePatientDto
    {


        // [Required]
        //[MaxLength(50)]
        //     public string MedicalRecordNumber { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? BloodType { get; set; }
    }
}
