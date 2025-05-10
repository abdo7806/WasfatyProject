// 📁 Models/Pharmacy.cs

public partial class Pharmacy// صيدلية
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<DispenseRecord> DispenseRecords { get; set; } = new List<DispenseRecord>();

    public virtual ICollection<Pharmacist> Pharmacists { get; set; } = new List<Pharmacist>();
}