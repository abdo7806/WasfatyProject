using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;
using Wasfaty.Domain.Entities.Audit;

namespace Wasfaty.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IAuditLogRepository auditLogRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string? entityId = null,
            string? details = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = httpContext?.User.FindFirstValue(ClaimTypes.Email);
                var userRole = httpContext?.User.FindFirstValue(ClaimTypes.Role);
                var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    UserEmail = userEmail,
                    UserRole = userRole,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    IpAddress = ipAddress,
                    UserAgent = !string.IsNullOrEmpty(userAgent) && userAgent.Length > 200
                                                 ? userAgent.Substring(0, 200)
                                                 : userAgent,
                    Details = details,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog);
                await _auditLogRepository.SaveChangesAsync();

                _logger.LogDebug("Audit log created: {Action} on {EntityName} (Id: {EntityId}) by User: {UserId}",
                    action, entityName, entityId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log for action: {Action}", action);
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsForEntityAsync(string entityName, string? entityId)
        {
            return await _auditLogRepository.GetByEntityAsync(entityName, entityId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsForUserAsync(string userId)
        {
            return await _auditLogRepository.GetByUserAsync(userId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _auditLogRepository.GetByDateRangeAsync(from, to);
        }

        public async Task<AuditLog?> GetAuditLogByIdAsync(int id)
        {
            return await _auditLogRepository.GetByIdAsync(id);
        }

        // Wasfaty.Infrastructure/Services/AuditService.cs (أضف هذه الدوال)

        public async Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? action = null,
            string? entityName = null,
            string? userId = null)
        {
            var query = _auditLogRepository.GetQueryable();

            if (!string.IsNullOrEmpty(action))
                query = query.Where(x => x.Action.Contains(action));

            if (!string.IsNullOrEmpty(entityName))
                query = query.Where(x => x.EntityName.Contains(entityName));

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(x => x.UserId == userId);

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, totalCount);
        }

        public async Task<AuditLog?> GetByIdAsync(long id)
        {
            return await _auditLogRepository.GetByIdAsync((int)id);
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int take = 100)
        {
            return await _auditLogRepository.GetByUserAsync(userId, take);
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId)
        {
            return await _auditLogRepository.GetByEntityAsync(entityName, entityId);
        }
    }
}
