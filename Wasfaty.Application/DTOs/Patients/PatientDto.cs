using Wasfaty.Application.DTOs.Prescriptions;
using Wasfaty.Application.DTOs.Users;

namespace Wasfaty.Application.DTOs.Patients
{
    public class PatientDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //   public string MedicalRecordNumber { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? BloodType { get; set; }


        public UserDto User { get; set; }
        public IEnumerable<PrescriptionDto> Prescriptions { get; set; } = new List<PrescriptionDto>();
    }
}
