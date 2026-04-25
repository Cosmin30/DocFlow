using DocFlow.ApprovalService.Application.Contracts;
using DocFlow.ApprovalService.Domain.Entities;
using DocFlow.ApprovalService.Infrastructure.Repositories;

namespace DocFlow.ApprovalService.Application.Services;

public sealed class ApprovalService(IApprovalRepository repository) : IApprovalService
{
    public async Task<ApprovalRequest> CreateAsync(Guid tenantId, Guid requestedByUserId, CreateApprovalRequest request, CancellationToken cancellationToken)
    {
        var approval = new ApprovalRequest
        {
            TenantId = tenantId,
            DocumentId = request.DocumentId,
            RequestedByUserId = requestedByUserId,
            AssignedToUserId = request.AssignedToUserId,
            Comment = request.Comment,
            Status = ApprovalStatus.Pending
        };

        await repository.AddAsync(approval, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return approval;
    }

    public Task<List<ApprovalRequest>> GetPendingAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken) =>
        repository.GetPendingForUserAsync(tenantId, userId, cancellationToken);

    public async Task<ApprovalRequest?> DecideAsync(Guid id, Guid tenantId, DecisionRequest request, CancellationToken cancellationToken)
    {
        var approval = await repository.GetByIdAsync(id, tenantId, cancellationToken);
        if (approval is null)
        {
            return null;
        }

        approval.Status = request.Approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        approval.Comment = request.Comment ?? approval.Comment;
        approval.ResolvedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        return approval;
    }
}
