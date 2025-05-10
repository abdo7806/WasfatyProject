// 📁 Models/Pharmacist.cs

public partial class Pharmacist// صيدلي
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PharmacyId { get; set; }

    public string? LicenseNumber { get; set; }

    public virtual ICollection<DispenseRecord> DispenseRecords { get; set; } = new List<DispenseRecord>();

    public virtual Pharmacy Pharmacy { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}