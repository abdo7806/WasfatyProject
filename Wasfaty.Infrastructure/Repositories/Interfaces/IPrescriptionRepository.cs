using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(int id);
    Task<List<Prescription>> GetAllAsync();
    Task<Prescription> AddAsync(Prescription prescription);
    Task<Prescription> UpdateAsync(Prescription prescription);
    Task<bool> DeleteAsync(int id);
}