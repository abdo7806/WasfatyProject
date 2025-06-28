// 📁 Models/PrescriptionItem.cs
public partial class PrescriptionItem//تفاصيل الأدوية داخل كل وصفة
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    // العلاقة الاختيارية مع الدواء

    public int? MedicationId { get; set; }

    // حقول الدواء المخصص
    public string? CustomMedicationName { get; set; }
    public string? CustomMedicationDescription { get; set; }
    public string? CustomDosageForm { get; set; }
    public string? CustomStrength { get; set; }

    public string? Dosage { get; set; }// الجرعه
    public string? Frequency { get; set; }// عدد مرات اليوم
    public string? Duration { get; set; }// مدة الاستخدام

    public virtual Medication? Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}

