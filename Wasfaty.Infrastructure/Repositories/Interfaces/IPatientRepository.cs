using Wasfaty.Application.DTOs.Patients;

public interface IPatientRepository
{
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task<Patient> GetByIdAsync(int id);
    Task<Patient> AddAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(int id);

    Task<List<Patient>> SearchPatients(string term);

    Task<Patient> GetPatientByUserIdAsync(int userId);

    Task<PatientDashboardDto> GetDashboardDataAsync(int patientId);//صفحة لوحة القيادة للمريض



}