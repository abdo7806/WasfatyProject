using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMedicationRepository
{
    Task<Medication> GetByIdAsync(int id);
    Task<List<Medication>> GetAllAsync();
    Task<Medication> AddAsync(Medication medication);
    Task<Medication> UpdateAsync(Medication medication);
    Task<bool> DeleteAsync(int id);
}