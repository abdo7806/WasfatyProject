using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DispenseRecordRepository : IDispenseRecordRepository
{
    private readonly ApplicationDbContext _context;

    public DispenseRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DispenseRecord?> GetByIdAsync(int id)
    {
        return await _context.DispenseRecords
            .Include(dr => dr.Pharmacist.User)
            .Include(dr => dr.Pharmacy)
            .Include(dr => dr.Prescription.Doctor.User)
            .Include(dr => dr.Prescription.Patient.User)

            .FirstOrDefaultAsync(dr => dr.Id == id);
    }

    public async Task<List<DispenseRecord>> GetAllAsync()
    {
        return await _context.DispenseRecords
            .Include(dr => dr.Pharmacist.User)
            .Include(dr => dr.Pharmacy)
            .Include(dr => dr.Prescription.Doctor.User)
            .Include(dr => dr.Prescription.Patient.User)
            .ToListAsync();
    }

    public async Task<DispenseRecord> AddAsync(DispenseRecord dispenseRecord, Prescription prescription)
    {


        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // إضافة سجل الصرف
                await _context.DispenseRecords.AddAsync(dispenseRecord);

                // تحديث حالة الوصفة
                prescription.IsDispensed = true; // تأكد من تحديث الحالة مباشرة
                _context.Entry(prescription).State = EntityState.Modified;

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // تأكيد المعاملة
                await transaction.CommitAsync();

                return dispenseRecord;
            }
            catch (Exception ex)
            {
                // إلغاء المعاملة في حالة حدوث خطأ
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return null;
            }
        }


    }

    public async Task<DispenseRecord> UpdateAsync(DispenseRecord dispenseRecord)
    {
        _context.Entry(dispenseRecord).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return dispenseRecord;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dispenseRecord = await GetByIdAsync(id);
        if (dispenseRecord != null)
        {
            _context.DispenseRecords.Remove(dispenseRecord);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<DispenseRecord>> GetByPharmacyIdAsync(int PharmacyId)
    {
        return await _context.DispenseRecords
            .Include(dr => dr.Pharmacist.User)
            .Include(dr => dr.Pharmacy)
            .Include(dr => dr.Prescription.Doctor.User)
            .Include(dr => dr.Prescription.Patient.User)
            .Include(dr => dr.Prescription.PrescriptionItems)
            .Where(p => p.PharmacyId == PharmacyId)
            .ToListAsync();
    }
}