using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Pharmacists;

public class PharmacistRepository : IPharmacistRepository
{
    private readonly ApplicationDbContext _context;

    public PharmacistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pharmacist?> GetByIdAsync(int id)
    {

       
        return await _context.Pharmacists
            .Include(p => p.DispenseRecords)
            .Include(d => d.Pharmacy)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Pharmacist>> GetAllAsync()
    {
        return await _context.Pharmacists
            .Include(p => p.DispenseRecords)
           .Include(d => d.Pharmacy)
           .Include(d => d.User)
           .ToListAsync();
    }

    public async Task<Pharmacist> AddAsync(Pharmacist pharmacist)
    {
        _context.Pharmacists.Add(pharmacist);
        await _context.SaveChangesAsync();
        return pharmacist;
    }

    public async Task<Pharmacist> UpdateAsync(Pharmacist pharmacist)
    {
        _context.Entry(pharmacist).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return pharmacist;
    }

    public async Task<bool> DeleteAsync(int id)
    {


        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {

                var pharmacist = await GetByIdAsync(id);

                if (pharmacist == null)
                {
                    return false; // الصيدلي غير موجود
                }

                var User = pharmacist.User;
                if (User == null)
                {
                    return false; // المستخدم غير موجود
                }

                _context.Pharmacists.Remove(pharmacist);

                _context.Users.Remove(User);




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


        /*  var pharmacist = await GetByIdAsync(id);
          if (pharmacist != null)
          {
              _context.Pharmacists.Remove(pharmacist);
              await _context.SaveChangesAsync();
              return true;
          }
          return false;*/
    }

    public async Task<List<Pharmacist>> GetByPharmacyIdAsync(int PharmacyId)
    {
        return await _context.Pharmacists
              .Include(p => p.DispenseRecords)
             .Include(d => d.Pharmacy)
             .Include(d => d.User)
             .Where(p => p.PharmacyId == PharmacyId)
             .ToListAsync();
    }

    public async Task<Pharmacist> GetPharmacyByUserIdAsync(int userId)
    {

        return await _context.Pharmacists
            .Include(p => p.DispenseRecords)
            .Include(d => d.Pharmacy)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<PharmacistDashboardStatsDto> GetPharmacistDataAsync(int pharmacistId)
    {
        if (pharmacistId <= 0)
        {
            return null;
        }
        var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        // var pharmacistId = GetCurrentPharmacistId(); // افترض أن لديك دالة لجلب ID الصيدلي
        var pharmacyId = await _context.Pharmacists
            .Where(p => p.Id == pharmacistId)
            .Select(p => p.PharmacyId)
            .FirstOrDefaultAsync();

        var stats = new PharmacistDashboardStatsDto
        {
            PendingPrescriptions =  await _context.Prescriptions//الوصفات الطبية المعلقة
                .CountAsync(p => !p.IsDispensed),

            DispensedPrescriptionsByPharmacy = await _context.DispenseRecords//الوصفات الطبية المُصرفة من قبل هاذا الصيدلي
                    .CountAsync(d => d.PharmacyId == pharmacyId &&
                                   d.PharmacistId == pharmacistId),

            DispensedPrescriptionsByPharmcist = await _context.DispenseRecords//الوصفات الطبية المُصرفة من قبل هاذا الصيدلي
                    .CountAsync(d => d.PharmacyId == pharmacyId),

            MonthlyMedications = await _context.DispenseRecords
                .Where(d => d.PharmacyId == pharmacyId && d.DispensedDate >= firstDayOfMonth)
                .Join(_context.PrescriptionItems,
                    d => d.PrescriptionId,
                    pi => pi.PrescriptionId,
                    (d, pi) => pi)
                .CountAsync(),

            TopMedications = await _context.DispenseRecords
                .Where(d => d.PharmacyId == pharmacyId)
                .Join(_context.PrescriptionItems,
                    d => d.PrescriptionId,
                    pi => pi.PrescriptionId,
                    (d, pi) => pi)
                .Join(_context.Medications,
                    pi => pi.MedicationId,
                    m => m.Id,
                    (pi, m) => new { m.Name, m.Id })
                .GroupBy(x => new { x.Id, x.Name })
                .Select(g => new TopMedicationDTO
                {
                    MedicationName = g.Key.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync()
        };



        return stats;
      
    }

    // GET: api/pharmacistdashboard/recent
  /*  [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<RecentPrescription>>> GetRecentPrescriptions()
    {
        var pharmacistId = GetCurrentPharmacistId();
        var today = DateTime.Today;

        var recent = await _context.Prescriptions
            .Where(p => p.PharmacyId == pharmacistId && p.CreatedAt.Date >= today.AddDays(-7))
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new RecentPrescription
            {
                Id = p.Id,
                PatientName = p.Patient.FullName,
                DoctorName = p.Doctor.FullName,
                Date = p.CreatedAt,
                Status = p.Status == "Dispensed" ? "مصروفة" : "معلقة",
                IsUrgent = p.IsUrgent
            })
            .ToListAsync();

        return Ok(recent);
    }
    */

}