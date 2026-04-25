using DocFlow.ApprovalService.Application.Contracts;
using DocFlow.ApprovalService.Domain.Entities;

namespace DocFlow.ApprovalService.Application.Services;

public interface IApprovalService
{
    Task<ApprovalRequest> CreateAsync(Guid tenantId, Guid requestedByUserId, CreateApprovalRequest request, CancellationToken cancellationToken);
    Task<List<ApprovalRequest>> GetPendingAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
    Task<ApprovalRequest?> DecideAsync(Guid id, Guid tenantId, DecisionRequest request, CancellationToken cancellationToken);
}
