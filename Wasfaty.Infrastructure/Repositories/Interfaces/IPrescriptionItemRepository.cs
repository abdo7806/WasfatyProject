using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPrescriptionItemRepository
{
    Task<PrescriptionItem?> GetByIdAsync(int id);
    Task<List<PrescriptionItem>> GetAllAsync();
    Task<PrescriptionItem> AddAsync(PrescriptionItem prescriptionItem);
    Task<PrescriptionItem> UpdateAsync(PrescriptionItem prescriptionItem);
    Task<bool> DeleteAsync(int id);
}