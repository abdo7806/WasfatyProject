public interface ICrudService<TDto, TCreateDto, TUpdateDto>
{
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto> CreateAsync(TCreateDto dto);
    Task<bool> UpdateAsync(int id, TUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
