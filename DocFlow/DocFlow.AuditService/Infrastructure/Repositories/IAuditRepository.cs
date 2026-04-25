using DocFlow.AuditService.Domain.Entities;

namespace DocFlow.AuditService.Infrastructure.Repositories;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);
    Task<List<AuditLog>> GetByTenantAsync(Guid tenantId, int take, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
