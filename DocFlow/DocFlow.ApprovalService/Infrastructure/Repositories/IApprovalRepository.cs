using DocFlow.ApprovalService.Domain.Entities;

namespace DocFlow.ApprovalService.Infrastructure.Repositories;

public interface IApprovalRepository
{
    Task AddAsync(ApprovalRequest request, CancellationToken cancellationToken);
    Task<List<ApprovalRequest>> GetPendingForUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
    Task<ApprovalRequest?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
