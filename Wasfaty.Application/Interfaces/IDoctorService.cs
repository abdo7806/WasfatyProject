using System.Collections.Generic;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Doctors;

namespace Wasfaty.Application.Interfaces
{
    public interface IDoctorService
    {
        /// <summary>
        /// إنشاء طبيب جديد (بما في ذلك إنشاء مستخدم)
        /// </summary>
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto);

        /// <summary>
        /// تحديث بيانات طبيب
        /// </summary>
        Task<DoctorDto> UpdateDoctorAsync(int doctorId, UpdateDoctorDto dto);

        /// <summary>
        /// حذف طبيب
        /// </summary>
        Task<bool> DeleteDoctorAsync(int doctorId);

        /// <summary>
        /// جلب كل الأطباء
        /// </summary>
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();

        /// <summary>
        /// جلب طبيب بواسطة المعرف
        /// </summary>
        Task<DoctorDto> GetDoctorByIdAsync(int doctorId);

        /// <summary>
        /// UserId جلب طبيب بواسطة المعرف المستخدم
        /// </summary>
        Task<DoctorDto> GetDoctorByUserIdAsync(int userId);
    }
}
