using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.MedicalCenters;

namespace Wasfaty.Application.Interfaces
{
    /// <summary>
    /// واجهة خدمات سجلات الصرف
    /// </summary>
    public interface IDispenseRecordService
    {
        /// <summary>
        /// إضافة سجل صرف جديد
        /// </summary>
        /// <param name="createDispenseRecordDto">بيانات سجل الصرف الجديد</param>
        /// <returns>تفاصيل سجل الصرف بعد إضافته</returns>
        Task<DispenseRecordDto> CreateAsync(CreateDispenseRecordDto createDispenseRecordDto);

        /// <summary>
        /// تعديل سجل صرف موجود
        /// </summary>
        /// <param name="id">معرف سجل الصرف</param>
        /// <param name="updateDispenseRecordDto">بيانات سجل الصرف المعدلة</param>
        /// <returns>تفاصيل سجل الصرف المعدل</returns>
        Task<DispenseRecordDto> UpdateAsync(int id, CreateDispenseRecordDto updateDispenseRecordDto);

        /// <summary>
        /// حذف سجل صرف
        /// </summary>
        /// <param name="id">معرف سجل الصرف</param>
        /// <returns>نتيجة عملية الحذف</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// استعراض جميع سجلات الصرف المرتبطة بوصفة طبية معينة
        /// </summary>
        /// <param name="prescriptionId">معرف الوصفة الطبية</param>
        /// <returns>قائمة سجلات الصرف</returns>
        Task<DispenseRecordDto?> GetByIdAsync(int prescriptionId);

        Task<List<DispenseRecordDto>> GetAllAsync();


    }
}
