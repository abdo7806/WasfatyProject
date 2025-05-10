using System.ComponentModel.DataAnnotations;

namespace Wasfaty.Application.DTOs.Patients
{
    public class UpdatePatientDto
    {


        // [Required]
        //[MaxLength(50)]
        //     public string MedicalRecordNumber { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? BloodType { get; set; }
    }
}
