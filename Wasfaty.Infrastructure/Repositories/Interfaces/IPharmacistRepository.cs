using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPharmacistRepository
{
    Task<Pharmacist?> GetByIdAsync(int id); 
    Task<List<Pharmacist>> GetByPharmacyIdAsync(int PharmacyId); 
    Task<List<Pharmacist>> GetAllAsync();
    Task<Pharmacist> AddAsync(Pharmacist pharmacist);
    Task<Pharmacist> UpdateAsync(Pharmacist pharmacist);
    Task<bool> DeleteAsync(int id);
}