public interface IPharmacyRepository
{
    Task<Pharmacy> GetByIdAsync(int id);
    Task<IEnumerable<Pharmacy>> GetAllAsync();
    Task<Pharmacy> AddAsync(Pharmacy pharmacy);
    Task<Pharmacy> UpdateAsync(Pharmacy pharmacy);
    Task<bool> DeleteAsync(int id);
}