using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PrescriptionItemRepository : IPrescriptionItemRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrescriptionItem?> GetByIdAsync(int id)
    {
        return await _context.PrescriptionItems
            .Include(pi => pi.Medication)
            .Include(pi => pi.Prescription)
            .FirstOrDefaultAsync(pi => pi.Id == id);
    }

    public async Task<List<PrescriptionItem>> GetAllAsync()
    {
        return await _context.PrescriptionItems
            .Include(pi => pi.Medication)
            .Include(pi => pi.Prescription)
            .ToListAsync();
    }

    public async Task<PrescriptionItem> AddAsync(PrescriptionItem prescriptionItem)
    {
        _context.PrescriptionItems.Add(prescriptionItem);
        await _context.SaveChangesAsync();
        return prescriptionItem;
    }

    public async Task<PrescriptionItem> UpdateAsync(PrescriptionItem prescriptionItem)
    {
        _context.Entry(prescriptionItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return prescriptionItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var prescriptionItem = await GetByIdAsync(id);
        if (prescriptionItem != null)
        {
            _context.PrescriptionItems.Remove(prescriptionItem);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}