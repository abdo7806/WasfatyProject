using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ApplicationDbContext : DbContext
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MedicalCenter> MedicalCenters { get; set; }
    public DbSet<Pharmacy> Pharmacies { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Pharmacist> Pharmacists { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public DbSet<DispenseRecord> DispenseRecords { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // جدول Roles
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);

        // جدول Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Doctor)
            .WithOne(d => d.User)
            .HasForeignKey<Doctor>(d => d.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Pharmacist)
            .WithOne(ph => ph.User)
            .HasForeignKey<Pharmacist>(ph => ph.UserId);

        // جدول Doctors
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.MedicalCenter)
            .WithMany(mc => mc.Doctors)
            .HasForeignKey(d => d.MedicalCenterId);

        // جدول Patients
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Prescriptions)
            .WithOne(pr => pr.Patient)
            .HasForeignKey(pr => pr.PatientId);

        // جدول Prescriptions
        modelBuilder.Entity<Prescription>()
            .HasOne(pr => pr.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(pr => pr.DoctorId);

        modelBuilder.Entity<Prescription>()
            .HasMany(pr => pr.PrescriptionItems)
            .WithOne(pi => pi.Prescription)
            .HasForeignKey(pi => pi.PrescriptionId);

        // جدول PrescriptionItems
        modelBuilder.Entity<PrescriptionItem>()
            .HasOne(pi => pi.Medication)
            .WithMany(m => m.PrescriptionItems)
            .HasForeignKey(pi => pi.MedicationId)
            .OnDelete(DeleteBehavior.SetNull); // SET NULL عند الحذف

        // تكوين القيد CHECK
        modelBuilder.Entity<PrescriptionItem>()
            .HasCheckConstraint(
                "CHK_PrescriptionItem_Medication",
                "([MedicationId] IS NOT NULL OR ([CustomMedicationName] IS NOT NULL AND [CustomMedicationDescription] IS NOT NULL))");


        // جدول DispenseRecords
        modelBuilder.Entity<DispenseRecord>()
            .HasOne(dr => dr.Prescription)
            .WithOne(p => p.DispenseRecord)
            .HasForeignKey<DispenseRecord>(dr => dr.PrescriptionId);

        modelBuilder.Entity<DispenseRecord>()
            .HasOne(dr => dr.Pharmacist)
            .WithMany(ph => ph.DispenseRecords)
            .HasForeignKey(dr => dr.PharmacistId);

        modelBuilder.Entity<DispenseRecord>()
            .HasOne(dr => dr.Pharmacy)
            .WithMany(ph => ph.DispenseRecords)
            .HasForeignKey(dr => dr.PharmacyId);

        // جدول Pharmacists
        modelBuilder.Entity<Pharmacist>()
            .HasOne(ph => ph.Pharmacy)
            .WithMany(p => p.Pharmacists)
            .HasForeignKey(ph => ph.PharmacyId);
    }
}
