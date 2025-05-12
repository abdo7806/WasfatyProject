using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MedicationRepository : IMedicationRepository
{
    private readonly ApplicationDbContext _context;

    public MedicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Medication> GetByIdAsync(int id)
    {
        return await _context.Medications
            .Include(m => m.PrescriptionItems)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Medication>> GetAllAsync()
    {
        return await _context.Medications
            .Include(m => m.PrescriptionItems)
            .ToListAsync();
    }

    public async Task<Medication> AddAsync(Medication medication)
    {
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<Medication> UpdateAsync(Medication medication)
    {
        _context.Entry(medication).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medication = await GetByIdAsync(id);
        if (medication != null)
        {
            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<List<Medication>> GetMedicationsByIdsAsync(List<int> ids)
    {
        return await _context.Medications
            .Include(m => m.PrescriptionItems)
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }
}