using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wasfaty.Infrastructure.Data;

namespace Wasfaty.Infrastructure.Services;

public class AuditCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval;
    private readonly int _retentionDays;

    public AuditCleanupService(
        IServiceProvider serviceProvider,
        ILogger<AuditCleanupService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _cleanupInterval = TimeSpan.FromHours(
            configuration.GetValue<int>(
                "AuditCleanup:IntervalHours",
                24));

        _retentionDays = configuration.GetValue<int>(
            "AuditCleanup:RetentionDays",
            365);
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Audit Cleanup Service started. Interval: {IntervalHours}h, Retention: {RetentionDays} days",
            _cleanupInterval.TotalHours,
            _retentionDays);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldAuditLogs(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to cleanup audit logs");
            }

            await Task.Delay(
                _cleanupInterval,
                stoppingToken);
        }

        _logger.LogInformation(
            "Audit Cleanup Service stopped");
    }

    private async Task CleanupOldAuditLogs(
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var cutoffDate = DateTime.UtcNow
            .AddDays(-_retentionDays);

        var deletedCount = await context.AuditLogs
            .Where(x => x.CreatedAtUtc < cutoffDate)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation(
                "Removed {Count} audit logs older than {RetentionDays} days",
                deletedCount,
                _retentionDays);
        }
        else
        {
            _logger.LogDebug(
                "No old audit logs found for cleanup");
        }
    }
}