using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PharmacyRepository : IPharmacyRepository
{
    private readonly ApplicationDbContext _context;

    public PharmacyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pharmacy> GetByIdAsync(int id)
    {
        return await _context.Pharmacies
            .Include(p => p.DispenseRecords)
            .Include(p => p.Pharmacists)
             .ThenInclude(ph => ph.User)
            .FirstOrDefaultAsync(p => p.Id == id);

    }

    public async Task<IEnumerable<Pharmacy>> GetAllAsync()
    {
        return await _context.Pharmacies
            .Include(p => p.DispenseRecords)
            .Include(p => p.Pharmacists)
             .ThenInclude(ph => ph.User) // تضمين الكيانات المرتبطة User

            .ToListAsync();
    }

    public async Task<Pharmacy> AddAsync(Pharmacy pharmacy)
    {
        _context.Pharmacies.Add(pharmacy);
        await _context.SaveChangesAsync();
        return pharmacy;
    }

    public async Task<Pharmacy> UpdateAsync(Pharmacy pharmacy)
    {
        //_context.Entry(pharmacy).State = EntityState.Modified;
        _context.Pharmacies.Update(pharmacy);
        await _context.SaveChangesAsync();
        return pharmacy;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pharmacy = await GetByIdAsync(id);
        if (pharmacy != null)
        {
            _context.Pharmacies.Remove(pharmacy);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}