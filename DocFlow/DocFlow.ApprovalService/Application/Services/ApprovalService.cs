using DocFlow.ApprovalService.Application.Contracts;
using DocFlow.ApprovalService.Domain.Entities;
using DocFlow.ApprovalService.Infrastructure.Repositories;
using DocFlow.BuildingBlocks.Messaging;
using DocFlow.BuildingBlocks.Messaging.Events;

namespace DocFlow.ApprovalService.Application.Services;

public sealed class ApprovalService(IApprovalRepository repository, IEventBus eventBus) : IApprovalService
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

        await eventBus.PublishAsync(
            new ApprovalRequestedIntegrationEvent(
                tenantId,
                approval.Id,
                approval.DocumentId,
                requestedByUserId,
                approval.AssignedToUserId,
                approval.Comment,
                DateTime.UtcNow),
            topicName: "docflow.approval.requested",
            cancellationToken);

        await eventBus.PublishAsync(
            new NotificationIntegrationEvent(
                tenantId,
                UserId: approval.AssignedToUserId,
                Title: "Approval requested",
                Message: $"You have a pending approval for document {approval.DocumentId}.",
                CreatedAtUtc: DateTime.UtcNow),
            topicName: "docflow.notifications",
            cancellationToken);

        return approval;
    }

    public Task<List<ApprovalRequest>> GetPendingAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken) =>
        repository.GetPendingForUserAsync(tenantId, userId, cancellationToken);

    public async Task<ApprovalRequest?> DecideAsync(Guid id, Guid tenantId, Guid decidedByUserId, DecisionRequest request, CancellationToken cancellationToken)
    {
        var approval = await repository.GetByIdAsync(id, tenantId, cancellationToken);
        if (approval is null)
        {
            return null;
        }

        if (approval.AssignedToUserId != decidedByUserId)
        {
            return null;
        }

        approval.Status = request.Approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        approval.Comment = request.Comment ?? approval.Comment;
        approval.ResolvedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(
            new ApprovalDecidedIntegrationEvent(
                tenantId,
                approval.Id,
                approval.DocumentId,
                decidedByUserId,
                request.Approve,
                approval.Comment,
                approval.ResolvedAtUtc.Value),
            topicName: "docflow.approval.decided",
            cancellationToken);

        await eventBus.PublishAsync(
            new NotificationIntegrationEvent(
                tenantId,
                UserId: approval.RequestedByUserId,
                Title: request.Approve ? "Approval approved" : "Approval rejected",
                Message: $"Approval for document {approval.DocumentId} was {(request.Approve ? "approved" : "rejected")}.",
                CreatedAtUtc: DateTime.UtcNow),
            topicName: "docflow.notifications",
            cancellationToken);

        return approval;
    }
}
