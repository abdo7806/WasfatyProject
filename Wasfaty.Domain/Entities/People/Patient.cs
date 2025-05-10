// 📁 Models/Patient.cs

public partial class Patient// المريض
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual User User { get; set; } = null!;
}
