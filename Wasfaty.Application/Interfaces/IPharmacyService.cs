using Wasfaty.Application.DTOs.Pharmacies;

namespace Wasfaty.Application.Interfaces
{
    public interface IPharmacyService
    {
        /// <summary>
        /// إنشاء صيدلية جديدة
        /// </summary>
        Task<PharmacyDto> CreateAsync(CreatePharmacyDto dto);

        /// <summary>
        /// تحديث بيانات صيدلية
        /// </summary>
        Task<PharmacyDto> UpdateAsync(int id, UpdatePharmacyDto dto);

        /// <summary>
        /// حذف صيدلية
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// الحصول على صيدلية بواسطة المعرف
        /// </summary>
        Task<PharmacyDto?> GetByIdAsync(int id);

        /// <summary>
        /// الحصول على كل الصيدليات
        /// </summary>
        Task<IEnumerable<PharmacyDto>> GetAllAsync();
    }
}
