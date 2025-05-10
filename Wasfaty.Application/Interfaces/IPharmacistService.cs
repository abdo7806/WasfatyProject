using Wasfaty.Application.DTOs.Pharmacists;

namespace Wasfaty.Application.Interfaces
{
    public interface IPharmacistService
    {
        /// <summary>
        /// إنشاء صيدلي جديد
        /// </summary>
        Task<PharmacistDto> CreateAsync(CreatePharmacistDto dto);

        /// <summary>
        /// تحديث بيانات صيدلي
        /// </summary>
        Task<PharmacistDto> UpdateAsync(int id, UpdatePharmacistDto dto);

        /// <summary>
        /// حذف صيدلي
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// الحصول على صيدلي بواسطة المعرف
        /// </summary>
        Task<PharmacistDto?> GetByIdAsync(int id);

        /// <summary>
        /// الحصول على كل الصيادلة
        /// </summary>
        Task<List<PharmacistDto>> GetAllAsync();


        /// <summary>
        /// الحصول على كل الصيادلة المرتبطة ربقم صيدلية معينة
        /// </summary>
        Task<List<PharmacistDto>> GetByPharmacyIdAsync(int PharmacyId);

    }
}
