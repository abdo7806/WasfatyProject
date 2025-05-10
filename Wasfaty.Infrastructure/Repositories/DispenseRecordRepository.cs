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

    public async Task<DispenseRecord> AddAsync(DispenseRecord dispenseRecord)
    {
        _context.DispenseRecords.Add(dispenseRecord);
        await _context.SaveChangesAsync();
        return dispenseRecord;
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
}