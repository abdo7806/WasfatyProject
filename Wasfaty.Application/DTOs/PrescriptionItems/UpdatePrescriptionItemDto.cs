namespace Wasfaty.Application.DTOs.Prescriptions
{
    public class UpdatePrescriptionItemDto
    {
        //public int PrescriptionId { get; set; }
        // إما MedicationId (لأدوية موجودة)
        public int? MedicationId { get; set; }

        // أو بيانات الدواء المخصص (لأدوية جديدة)
        public string? CustomMedicationName { get; set; }
        public string? CustomMedicationDescription { get; set; }
        public string? CustomDosageForm { get; set; }
        public string? CustomStrength { get; set; }

        // تفاصيل الجرعة
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
    }
}