using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDispenseRecordRepository
{
    Task<DispenseRecord?> GetByIdAsync(int id);
    Task<List<DispenseRecord>> GetAllAsync();
    Task<DispenseRecord> AddAsync(DispenseRecord dispenseRecord);
    Task<DispenseRecord> UpdateAsync(DispenseRecord dispenseRecord);
    Task<bool> DeleteAsync(int id);
}