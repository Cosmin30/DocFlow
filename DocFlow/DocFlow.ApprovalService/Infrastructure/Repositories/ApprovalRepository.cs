using DocFlow.ApprovalService.Domain.Entities;
using DocFlow.ApprovalService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.ApprovalService.Infrastructure.Repositories;

public sealed class ApprovalRepository(ApprovalDbContext dbContext) : IApprovalRepository
{
    public async Task AddAsync(ApprovalRequest request, CancellationToken cancellationToken)
    {
        await dbContext.Approvals.AddAsync(request, cancellationToken);
    }

    public Task<List<ApprovalRequest>> GetPendingForUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken) =>
        dbContext.Approvals
            .Where(x => x.TenantId == tenantId && x.AssignedToUserId == userId && x.Status == ApprovalStatus.Pending)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<ApprovalRequest?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken) =>
        dbContext.Approvals.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
