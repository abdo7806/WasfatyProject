using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Prescriptions;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Prescription?> GetByIdAsync(int id)
    {
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
            .Include(p => p.Patient.User)
                        .Include(p => p.Doctor.MedicalCenter)

            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Prescription>> GetAllAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
                        .Include(p => p.Doctor.MedicalCenter)

            .Include(p => p.Patient.User)
            .ToListAsync();
    }

    public async Task<Prescription> AddAsync(Prescription prescription)
    {
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<Prescription> UpdateAsync(Prescription prescription)
    {
        _context.Entry(prescription).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<bool>DeleteAsync(int id)
    {

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var prescription = await GetByIdAsync(id);

                if (prescription == null)
                {
                    return false; // الوصفة غير موجودة
                }

                var prescriptionItems = prescription.PrescriptionItems;
                if(prescriptionItems == null || !prescriptionItems.Any())
                {
                    return false; // لا توجد عناصر في الوصفة
                }

                foreach ( var prescriptionItem in prescriptionItems)
                {
                    if (prescriptionItem != null)
                    {
                        _context.PrescriptionItems.Remove(prescriptionItem);
                    }
                }

                if (prescription != null)
                {
                    _context.Prescriptions.Remove(prescription);
                }

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // تأكيد المعاملة
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // إلغاء المعاملة في حالة حدوث خطأ
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return false;
            }
        }


    

    }

    public async Task<List<Prescription>> GetByDoctorIdAsync(int doctorId)
    {

  
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
            .Include(p => p.Patient.User)
            .Include(p => p.Doctor.MedicalCenter)
            .Where(p => p.DoctorId == doctorId)
            .ToListAsync(); ;
    }

    public async Task<List<Prescription>> GetByPatientIdAsync(int PatientId)
    {
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
            .Include(p => p.Doctor.MedicalCenter)
            .Include(p => p.Patient.User)
            
            .Where(p => p.PatientId == PatientId)
            .ToListAsync();
    }

    public async Task<PrescriptiontDashboardDto> GetDashboardDataAsync()
    {
        var DoctorCount = _context.Doctors.Count();

        var PharmacistCount = _context.Pharmacists.Count();

        var PatientCount = _context.Patients.Count();

        var PrescriptiontCount = _context.Prescriptions.Count();

        return new PrescriptiontDashboardDto
        {
            DoctorCount = DoctorCount,
            PatientCount = PatientCount,
            PharmacistCount = PharmacistCount,
            PrescriptiontCount = PrescriptiontCount
        };

    }

    public async Task<List<Prescription>> GetAllPrescriptionPendingAsync()
    {
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
                        .Include(p => p.Doctor.MedicalCenter)

            .Include(p => p.Patient.User)
            .Where(p => !p.IsDispensed)
            .ToListAsync();
    }

    public async Task<List<Prescription>> GetNewPrescriptionsAsync(int lastPrescriptionId)
    {
        return await _context.Prescriptions
               .Include(p => p.PrescriptionItems)
            .Include(p => p.Doctor.User)
             .Include(p => p.Doctor.MedicalCenter)
            .Include(p => p.Patient.User)
            .Where(p => p.Id > lastPrescriptionId && !p.IsDispensed)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
}