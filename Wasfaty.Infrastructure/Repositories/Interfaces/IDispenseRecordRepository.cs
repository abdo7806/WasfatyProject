using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.DispenseRecords;

public interface IDispenseRecordRepository
{
    Task<DispenseRecord?> GetByIdAsync(int id);
    Task<List<DispenseRecord>> GetAllAsync();
    Task<DispenseRecord> AddAsync(DispenseRecord dispenseRecord,Prescription prescription);
    Task<DispenseRecord> UpdateAsync(DispenseRecord dispenseRecord);
    Task<bool> DeleteAsync(int id);


    // ارجاع جميع الصرف حسب رقم الصيدليه 
    Task<List<DispenseRecord>> GetByPharmacyIdAsync(int PharmacyId);


}