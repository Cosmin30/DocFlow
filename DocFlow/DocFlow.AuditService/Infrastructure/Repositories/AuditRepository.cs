using DocFlow.AuditService.Domain.Entities;
using DocFlow.AuditService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuditService.Infrastructure.Repositories;

public sealed class AuditRepository(AuditDbContext dbContext) : IAuditRepository
{
    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    }

    public Task<List<AuditLog>> GetByTenantAsync(Guid tenantId, int take, CancellationToken cancellationToken) =>
        dbContext.AuditLogs
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
