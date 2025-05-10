using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.DTOs.Medications
{
    public class MedicationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
       public ICollection<PrescriptionItemDto> PrescriptionItems { get; set; } = new List<PrescriptionItemDto>();
    }
}
