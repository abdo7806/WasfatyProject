using Wasfaty.Application.DTOs.Medications;

namespace Wasfaty.Application.DTOs.Prescriptions
{

    public class PrescriptionItemDto
    {

        public int Id { get; set; }
        public int PrescriptionId { get; set; }

        // إما MedicationId (لأدوية موجودة)
        public int? MedicationId { get; set; }

        // أو بيانات الدواء المخصص (لأدوية جديدة)
        public string? CustomMedicationName { get; set; }
        public string? CustomMedicationDescription { get; set; }
        public string? CustomDosageForm { get; set; }
        public string? CustomStrength { get; set; }

        // معلومات الجرعة

        public string? Dosage { get; set; }// الجرعه
        public string? Frequency { get; set; }// عدد مرات اليوم
        public string? Duration { get; set; }// مدة الاستخدام
                                             // public string? MedicationName { get; set; }// مدة الاستخدام


        public MedicationDto? Medication { get; set; } = null!;


        public virtual PrescriptionDto Prescription { get; set; } = null!;

        // خاصية محسوبة لاسم الدواء
        public string MedicationName =>
            MedicationId.HasValue ? Medication?.Name ?? "غير معروف" : CustomMedicationName ?? "دواء غير محدد";

        // خاصية محسوبة لوصف الدواء
        public string MedicationDescription =>
            MedicationId.HasValue ? Medication?.Description ?? "لا يوجد وصف" : CustomMedicationDescription ?? "لا يوجد وصف";


    }
}
