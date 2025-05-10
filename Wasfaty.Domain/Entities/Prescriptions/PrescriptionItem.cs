// 📁 Models/PrescriptionItem.cs
public partial class PrescriptionItem//تفاصيل الأدوية داخل كل وصفة
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public int MedicationId { get; set; }
    public string? Dosage { get; set; }// الجرعه
    public string? Frequency { get; set; }// عدد مرات اليوم
    public string? Duration { get; set; }// مدة الاستخدام

    public virtual Medication Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}

