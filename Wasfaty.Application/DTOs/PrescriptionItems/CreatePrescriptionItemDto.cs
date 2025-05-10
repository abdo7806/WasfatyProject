namespace Wasfaty.Application.DTOs.Prescriptions
{
    public class CreatePrescriptionItemDto
    {
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
    }
}
