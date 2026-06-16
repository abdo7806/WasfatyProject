using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Domain.Entities.Audit;

namespace Wasfaty.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string? entityId)
        {
            var query = _context.AuditLogs.Where(x => x.EntityName == entityName);

            if (!string.IsNullOrEmpty(entityId))
                query = query.Where(x => x.EntityId == entityId);

            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId)
        {
            return await _context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.AuditLogs
                .Where(x => x.CreatedAtUtc >= from && x.CreatedAtUtc <= to)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
        {
            return await _context.AuditLogs.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<AuditLog> GetQueryable()
        {
            return _context.AuditLogs.AsQueryable();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, int? take = null)
        {
            IOrderedQueryable<AuditLog> query = _context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc);

            if (take.HasValue)
                query = query.Take(take.Value).OrderByDescending(x => x.CreatedAtUtc);

            return await query.ToListAsync();
        }
    }
}
