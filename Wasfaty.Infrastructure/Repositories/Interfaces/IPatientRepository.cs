using Wasfaty.Application.DTOs.Patients;

public interface IPatientRepository
{
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task<Patient> GetByIdAsync(int id);
    Task<Patient> AddAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(int id);

}