using DocFlow.AuditService.Application.Contracts;
using DocFlow.AuditService.Domain.Entities;
using DocFlow.AuditService.Infrastructure.Repositories;

namespace DocFlow.AuditService.Application.Services;

public sealed class AuditService(IAuditRepository repository) : IAuditService
{
    public async Task<AuditLog> WriteAsync(
        Guid tenantId,
        Guid? userId,
        string? ipAddress,
        string? device,
        WriteAuditRequest request,
        CancellationToken cancellationToken)
    {
        var log = new AuditLog
        {
            TenantId = tenantId,
            UserId = userId,
            Action = request.Action,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            MetadataJson = request.MetadataJson,
            IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress,
            Device = string.IsNullOrWhiteSpace(device) ? "unknown" : device
        };

        await repository.AddAsync(log, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return log;
    }

    public Task<List<AuditLog>> ListAsync(Guid tenantId, int take, CancellationToken cancellationToken) =>
        repository.GetByTenantAsync(tenantId, take, cancellationToken);
}
