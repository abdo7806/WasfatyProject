using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Domain.Entities.Accounts;
using Wasfaty.Domain.Entities.Audit;

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
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

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

        modelBuilder.Entity<User>()
           .HasMany(u => u.RefreshTokens)
           .WithOne(rt => rt.User)
           .HasForeignKey(rt => rt.UserId);

        // جدول Doctors
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.MedicalCenter)
            .WithMany(mc => mc.Doctors)
            .HasForeignKey(d => d.MedicalCenterId);

        // جدول Patients
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Prescriptions)
            .WithOne(pr => pr.Patient)
            .HasForeignKey(pr => pr.PatientId)
    .OnDelete(DeleteBehavior.NoAction);

        // جدول Prescriptions
        modelBuilder.Entity<Prescription>()
    .HasOne(pr => pr.Doctor)
    .WithMany(d => d.Prescriptions)
    .HasForeignKey(pr => pr.DoctorId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Prescription>()
            .HasMany(pr => pr.PrescriptionItems)
            .WithOne(pi => pi.Prescription)
            .HasForeignKey(pi => pi.PrescriptionId);

        // جدول PrescriptionItems
        modelBuilder.Entity<Prescription>()
            .HasMany(pr => pr.PrescriptionItems)
            .WithOne(pi => pi.Prescription)
            .HasForeignKey(pi => pi.PrescriptionId)
            .OnDelete(DeleteBehavior.NoAction);

        // تكوين القيد CHECK
        //modelBuilder.Entity<PrescriptionItem>()
        //    .HasCheckConstraint(
        //        "CHK_PrescriptionItem_Medication",
        //        "([MedicationId] IS NOT NULL OR ([CustomMedicationName] IS NOT NULL AND [CustomMedicationDescription] IS NOT NULL))");


        // جدول DispenseRecords
            modelBuilder.Entity<DispenseRecord>()
                .HasOne(dr => dr.Prescription)
                .WithOne(p => p.DispenseRecord)
                .HasForeignKey<DispenseRecord>(dr => dr.PrescriptionId)
        .OnDelete(DeleteBehavior.NoAction);

        //    modelBuilder.Entity<DispenseRecord>()
        //.HasOne(dr => dr.Prescription)
        //.WithMany(p => p.DispenseRecords)
        //.HasForeignKey(dr => dr.PrescriptionId)
        //.OnDelete(DeleteBehavior.NoAction);

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


        modelBuilder.Entity<RefreshToken>()
         .HasIndex(x => x.Token)
         .IsUnique();



        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
         .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        modelBuilder.Entity<User>()
      .HasMany(u => u.RefreshTokens)
      .WithOne(rt => rt.User)
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditLog>().Property(x => x.Action)
    .HasMaxLength(100);

        modelBuilder.Entity<AuditLog>().Property(x => x.EntityName)
            .HasMaxLength(100);

        // القيد اللي عندك
        modelBuilder.Entity<PrescriptionItem>()
            .HasCheckConstraint("CHK_PrescriptionItem_Medication",
                "([MedicationId] IS NOT NULL OR ([CustomMedicationName] IS NOT NULL AND [CustomMedicationDescription] IS NOT NULL))");

    }
}
