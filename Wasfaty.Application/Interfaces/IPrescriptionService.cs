using Wasfaty.Application.DTOs.DispenseRecords;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Prescriptions;

namespace Wasfaty.Application.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto);
        Task<PrescriptionDto> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionDto>> GetAllAsync();
        Task<PrescriptionDto> UpdateAsync(int id, CreatePrescriptionDto prescriptionDto);
        Task<bool> DeleteAsync(int id);

        Task<List<PrescriptionDto>> GetByDoctorIdAsync(int doctorId);


        Task<List<PrescriptionDto>> GetByPatientIdAsync(int PatientId);


        Task<PrescriptiontDashboardDto> GetDashboardDataAsync();//صفحة لوحة القيادة للادمن

        Task<List<PrescriptionDto>> GetAllPrescriptionPendingAsync();// ارجاع الوصفات الطبيه الذي قيد الانتظار



        // ارجاع اخر الوصفات الطبيه غير المصروفه
        Task<List<PrescriptionDto>> GetNewPrescriptionsAsync(int lastPrescriptionId);



    }

}
