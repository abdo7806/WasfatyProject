using Wasfaty.Application.DTOs.Medications;

namespace Wasfaty.Application.DTOs.Prescriptions
{
    
    public class PrescriptionItemDto
    {

        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }
        public string? Dosage { get; set; }// الجرعه
        public string? Frequency { get; set; }// عدد مرات اليوم
        public string? Duration { get; set; }// مدة الاستخدام
       // public string? MedicationName { get; set; }// مدة الاستخدام
        public  MedicationDto Medication { get; set; } = null!;


         public virtual PrescriptionDto Prescription { get; set; } = null!;

    }
}
