using System.Collections.Generic;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.DTOs.Patients;

namespace Wasfaty.Application.Interfaces
{
    public interface IPatientService
    {
        /// <summary>
        /// إنشاء مريض جديد
        /// </summary>
        Task<PatientDto> CreateAsync(CreatePatientDto dto);

        /// <summary>
        /// تحديث بيانات مريض
        /// </summary>
        Task<PatientDto> UpdateAsync(int id, UpdatePatientDto dto);

        /// <summary>
        /// الحصول على بيانات مريض محدد
        /// </summary>
        Task<PatientDto> GetByIdAsync(int id);

        /// <summary>
        /// الحصول على قائمة المرضى
        /// </summary>
        Task<IEnumerable<PatientDto>> GetAllAsync();

        /// <summary>
        /// حذف مريض
        /// </summary>
        Task<bool> DeleteAsync(int id);

        Task<List<PatientDto>> SearchPatients(string term);

        /// <summary>
        /// UserId جلب المريض بواسطة المعرف المستخدم
        /// </summary>
        Task<PatientDto> GetPatientByUserIdAsync(int userId);


        Task<PatientDashboardDto> GetDashboardDataAsync(int patientId);//صفحة لوحة القيادة للمريض
    }
}
