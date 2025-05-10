
public partial class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MedicalCenterId { get; set; }

    public string? Specialization { get; set; }

    public string? LicenseNumber { get; set; }

    public virtual MedicalCenter MedicalCenter { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual User User { get; set; } = null!;
}