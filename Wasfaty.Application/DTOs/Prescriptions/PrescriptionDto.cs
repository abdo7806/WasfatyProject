using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.Application.DTOs.Prescriptions
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public bool IsDispensed { get; set; }
        public DoctorDto Doctor { get; set; }
        public PatientDto Patient { get; set; }

        public List<PrescriptionItemDto> PrescriptionItems { get; set; } = new List<PrescriptionItemDto>();
    }
}
