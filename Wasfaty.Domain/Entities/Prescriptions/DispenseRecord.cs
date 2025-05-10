// 📁 Models/DispenseRecord.cs
public partial class DispenseRecord
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }

    public int PharmacistId { get; set; }

    public int PharmacyId { get; set; }

    public DateTime DispensedDate { get; set; }

    public virtual Pharmacist Pharmacist { get; set; } = null!;

    public virtual Pharmacy Pharmacy { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}

