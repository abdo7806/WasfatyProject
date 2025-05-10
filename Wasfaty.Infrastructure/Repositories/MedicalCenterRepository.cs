using Microsoft.EntityFrameworkCore;
using Wasfaty.Application.DTOs.MedicalCenters;

public class MedicalCenterRepository : IMedicalCenterRepository
{
    private readonly ApplicationDbContext _context;

    public MedicalCenterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MedicalCenter>> GetAllAsync()
    {

        return await _context.MedicalCenters
            .Include(mc => mc.Doctors) // تضمين الأطباء إذا لزم الأمر
            .ToListAsync();
    }

    public async Task<MedicalCenter> GetByIdAsync(int id)
    {
        return await _context.MedicalCenters
            .Include(mc => mc.Doctors) // تضمين الأطباء إذا لزم الأمر
            .FirstOrDefaultAsync(mc => mc.Id == id);
    }

    public async Task<MedicalCenter> AddAsync(MedicalCenter medicalCenter)
    {

        await _context.MedicalCenters.AddAsync(medicalCenter);
        await _context.SaveChangesAsync();
        return medicalCenter;
    }

    public async Task<MedicalCenter> UpdateAsync(MedicalCenter medicalCenter)
    {
        _context.MedicalCenters.Update(medicalCenter);
        await _context.SaveChangesAsync();
        return medicalCenter;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var medicalCenter = await GetByIdAsync(id);
            if (medicalCenter != null)
            {
                _context.MedicalCenters.Remove(medicalCenter);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
}