using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Medications;

public interface IMedicationRepository
{
    Task<Medication> GetByIdAsync(int id);
    Task<List<Medication>> GetAllAsync();
    Task<Medication> AddAsync(Medication medication);
    Task<Medication> UpdateAsync(Medication medication);
    Task<bool> DeleteAsync(int id);


    /// <summary>
    /// الحصول على كل الأدوية من خلال استقبال قائمة بارقام الادوية التي رح ترجع
    /// </summary>
    Task<List<Medication>> GetMedicationsByIdsAsync(List<int> ids);
}