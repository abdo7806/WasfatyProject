namespace Wasfaty.Application.DTOs.Prescriptions
{
    public class CreatePrescriptionDto
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsDispensed { get; set; }
     //   public List<CreatePrescriptionItemDto> PrescriptionItems { get; set; } = new List<CreatePrescriptionItemDto>();
    }
}
