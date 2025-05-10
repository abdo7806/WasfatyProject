using Wasfaty.Application.DTOs.Medications;

namespace Wasfaty.Application.Interfaces
{
    public interface IMedicationService
    {
        /// <summary>
        /// إنشاء دواء جديد
        /// </summary>
        Task<MedicationDto> CreateAsync(CreateMedicationDto dto);

        /// <summary>
        /// تحديث بيانات دواء
        /// </summary>
        Task<MedicationDto> UpdateAsync(int id, UpdateMedicationDto dto);

        /// <summary>
        /// حذف دواء
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// الحصول على دواء بواسطة المعرف
        /// </summary>
        Task<MedicationDto?> GetByIdAsync(int id);

        /// <summary>
        /// الحصول على كل الأدوية
        /// </summary>
        Task<List<MedicationDto>> GetAllAsync();
    }
}
