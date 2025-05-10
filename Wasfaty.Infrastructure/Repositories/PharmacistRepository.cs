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
        var pharmacist = await GetByIdAsync(id);
        if (pharmacist != null)
        {
            _context.Pharmacists.Remove(pharmacist);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
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
}