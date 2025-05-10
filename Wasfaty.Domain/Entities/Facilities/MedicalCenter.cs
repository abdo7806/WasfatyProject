// 📁 Models/MedicalCenter.cs
public partial class MedicalCenter// المركز الطبي
{

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
