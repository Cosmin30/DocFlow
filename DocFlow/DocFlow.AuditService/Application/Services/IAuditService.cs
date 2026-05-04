using DocFlow.AuditService.Application.Contracts;
using DocFlow.AuditService.Domain.Entities;

namespace DocFlow.AuditService.Application.Services;

public interface IAuditService
{
    Task<AuditLog> WriteAsync(Guid tenantId, Guid? userId, string? ipAddress, string? device, WriteAuditRequest request, CancellationToken cancellationToken);
    Task<List<AuditLog>> ListAsync(Guid tenantId, int take, CancellationToken cancellationToken);
}
