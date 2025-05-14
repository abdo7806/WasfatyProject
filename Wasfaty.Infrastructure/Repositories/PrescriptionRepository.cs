using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
        var prescription = await GetByIdAsync(id);
        if (prescription != null)
        {
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
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
}