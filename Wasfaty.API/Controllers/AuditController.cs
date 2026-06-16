// Wasfaty.API/Controllers/AuditController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Constants;
using Wasfaty.Application.Interfaces.IServices;

namespace Wasfaty.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(
        IAuditService auditService,
        ILogger<AuditController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// الحصول على جميع سجلات المراجعة مع تصفية وترقيم
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? action = null,
        [FromQuery] string? entityName = null,
        [FromQuery] string? userId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        var (logs, totalCount) = await _auditService.GetPagedAsync(
            page, pageSize, action, entityName, userId);

        return Ok(new
        {
            success = true,
            data = new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                logs
            }
        });
    }

    /// <summary>
    /// الحصول على سجل مراجعة محدد بالمعرف
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuditLogById(long id)
    {
        var log = await _auditService.GetByIdAsync(id);
        if (log == null)
            return NotFound(new { success = false, message = $"Audit log with ID {id} not found" });

        return Ok(new { success = true, data = log });
    }

    /// <summary>
    /// الحصول على سجلات مراجعة مستخدم محدد
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(string userId, [FromQuery] int take = 100)
    {
        if (take > 500) take = 500;

        var logs = await _auditService.GetByUserIdAsync(userId, take);

        return Ok(new { success = true, data = logs });
    }

    /// <summary>
    /// الحصول على سجلات مراجعة لكيان محدد (مثل Prescription/123)
    /// </summary>
    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<IActionResult> GetEntityAuditLogs(string entityName, string entityId)
    {
        var logs = await _auditService.GetByEntityAsync(entityName, entityId);

        return Ok(new { success = true, data = logs });
    }
}