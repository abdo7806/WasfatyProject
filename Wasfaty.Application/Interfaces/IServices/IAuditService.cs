using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Domain.Entities.Audit;

namespace Wasfaty.Application.Interfaces.IServices
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityName, string? entityId = null, string? details = null);

        // دوال الاستعلام (GET)
        Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? action = null,
            string? entityName = null,
            string? userId = null);

        Task<AuditLog?> GetByIdAsync(long id);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int take = 100);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId);
    }
}
