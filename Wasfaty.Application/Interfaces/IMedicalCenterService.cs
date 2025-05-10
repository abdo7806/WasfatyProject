using Wasfaty.Application.DTOs.MedicalCenters;

namespace Wasfaty.Application.Interfaces
{
    public interface IMedicalCenterService
    {
        /// <summary>
        /// إنشاء مركز طبي جديد
        /// </summary>
        Task<MedicalCenterDto> CreateAsync(CreateMedicalCenterDto dto);

        /// <summary>
        /// تحديث بيانات مركز طبي
        /// </summary>
        Task<MedicalCenterDto> UpdateAsync(int id, UpdateMedicalCenterDto dto);

        /// <summary>
        /// حذف مركز طبي
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// الحصول على مركز طبي بواسطة المعرف
        /// </summary>
        Task<MedicalCenterDto?> GetByIdAsync(int id);

        /// <summary>
        /// الحصول على كل المراكز الطبية
        /// </summary>
        Task<IEnumerable<MedicalCenterDto>> GetAllAsync();
    }
}
