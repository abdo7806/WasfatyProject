using Wasfaty.Domain.Entities.Audit;

namespace Wasfaty.Application.Interfaces.IRepositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task SaveChangesAsync();

        // دوال الاستعلام الجديدة
        IQueryable<AuditLog> GetQueryable();
        Task<AuditLog?> GetByIdAsync(int id);
        Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, int? take = null);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string? entityId);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}