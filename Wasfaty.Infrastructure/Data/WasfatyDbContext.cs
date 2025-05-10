using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Wasfaty.Infrastructure.Data;

public partial class WasfatyDbContext : DbContext
{
    public WasfatyDbContext()
    {
    }

    public WasfatyDbContext(DbContextOptions<WasfatyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DispenseRecord> DispenseRecords { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<MedicalCenter> MedicalCenters { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Pharmacist> Pharmacists { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionItem> PrescriptionItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=WasfatyDb;User Id=sa;Password=123456;Trusted_Connection=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DispenseRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dispense__3214EC07CCDA8E5D");

            entity.HasIndex(e => e.PrescriptionId, "UQ__Dispense__401308330FB4B853").IsUnique();

            entity.Property(e => e.DispensedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Pharmacist).WithMany(p => p.DispenseRecords)
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DispenseR__Pharm__5DCAEF64");

            entity.HasOne(d => d.Pharmacy).WithMany(p => p.DispenseRecords)
                .HasForeignKey(d => d.PharmacyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DispenseR__Pharm__5EBF139D");

            entity.HasOne(d => d.Prescription).WithOne(p => p.DispenseRecord)
                .HasForeignKey<DispenseRecord>(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DispenseR__Presc__5CD6CB2B");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctors__3214EC070CFA56E3");

            entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CC4DD96009DA").IsUnique();

            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.Specialization).HasMaxLength(100);

            entity.HasOne(d => d.MedicalCenter).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.MedicalCenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctors__Medical__440B1D61");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctors__UserId__4316F928");
        });

        modelBuilder.Entity<MedicalCenter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MedicalC__3214EC076E32305D");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medicati__3214EC072DB97CD5");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DosageForm).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Strength).HasMaxLength(50);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patients__3214EC07B31B7F9C");

            entity.HasIndex(e => e.UserId, "UQ__Patients__1788CC4D9F1930FD").IsUnique();

            entity.Property(e => e.BloodType).HasMaxLength(5);
            entity.Property(e => e.Gender).HasMaxLength(10);

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patients__UserId__47DBAE45");
        });

        modelBuilder.Entity<Pharmacist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pharmaci__3214EC07D26DD6E3");

            entity.HasIndex(e => e.UserId, "UQ__Pharmaci__1788CC4DC92F9FDF").IsUnique();

            entity.Property(e => e.LicenseNumber).HasMaxLength(50);

            entity.HasOne(d => d.Pharmacy).WithMany(p => p.Pharmacists)
                .HasForeignKey(d => d.PharmacyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pharmacis__Pharm__4CA06362");

            entity.HasOne(d => d.User).WithOne(p => p.Pharmacist)
                .HasForeignKey<Pharmacist>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pharmacis__UserI__4BAC3F29");
        });

        modelBuilder.Entity<Pharmacy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pharmaci__3214EC077C6B8971");

            entity.ToTable("Pharmacy");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prescrip__3214EC07042B5449");

            entity.Property(e => e.IssuedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Docto__534D60F1");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Patie__5441852A");
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prescrip__3214EC0748131A7A");

            entity.Property(e => e.Dosage).HasMaxLength(100);
            entity.Property(e => e.Duration).HasMaxLength(50);
            entity.Property(e => e.Frequency).HasMaxLength(50);

            entity.HasOne(d => d.Medication).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Medic__628FA481");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.PrescriptionId)
                .HasConstraintName("FK__Prescript__Presc__619B8048");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC076EBA56AF");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07AC125AD6");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105348F033944").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
