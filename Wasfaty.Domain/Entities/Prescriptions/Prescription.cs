// 📁 Models/Prescription.cs
public partial class Prescription//وصفة طبية
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public DateTime IssuedDate { get; set; }

    public bool IsDispensed { get; set; }

    public virtual DispenseRecord? DispenseRecord { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}
