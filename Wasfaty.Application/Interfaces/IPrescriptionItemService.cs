using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.Interfaces
{
    /// <summary>
    /// واجهة لخدمات تفاصيل الأدوية في الوصفات الطبية
    /// </summary>
    public interface IPrescriptionItemService
    {
        /// <summary>
        /// إضافة تفاصيل دواء إلى وصفة طبية
        /// </summary>
        /// <param name="createPrescriptionItemDto">بيانات الدواء المراد إضافته</param>
        /// <returns>تفاصيل الدواء بعد إضافته</returns>
        Task<PrescriptionItemDto> CreateAsync(CreatePrescriptionItemDto createPrescriptionItemDto);

        /// <summary>
        /// تعديل تفاصيل دواء في الوصفة الطبية
        /// </summary>
        /// <param name="id">معرف تفاصيل الدواء</param>
        /// <param name="updatePrescriptionItemDto">بيانات الدواء المعدلة</param>
        /// <returns>تفاصيل الدواء المعدلة</returns>
        Task<PrescriptionItemDto> UpdateAsync(int id, UpdatePrescriptionItemDto updatePrescriptionItemDto);

        /// <summary>
        /// حذف تفاصيل دواء من الوصفة الطبية
        /// </summary>
        /// <param name="id">معرف تفاصيل الدواء</param>
        /// <returns>نتيجة عملية الحذف</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// استعراض تفاصيل الأدوية في وصفة طبية معينة
        /// </summary>
        /// <param name="prescriptionId">معرف الوصفة الطبية</param>
        /// <returns>قائمة تفاصيل الأدوية في الوصفة</returns>
        Task<List<PrescriptionItemDto>> GetAllByPrescriptionId(int prescriptionId);
        Task<PrescriptionItemDto> GetByIdAsync(int prescriptionIdItem);

        Task<List<PrescriptionItemDto>> GetAllAsync();

    }
}
