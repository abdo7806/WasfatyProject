// 📁 Models/Medication.cs

public partial class Medication//دواء
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? DosageForm { get; set; }

    public string? Strength { get; set; }

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}