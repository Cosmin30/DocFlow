namespace DocFlow.BuildingBlocks.Messaging.Events;

public sealed record ApprovalRequestedIntegrationEvent(
    Guid TenantId,
    Guid ApprovalId,
    Guid DocumentId,
    Guid RequestedByUserId,
    Guid AssignedToUserId,
    string? Comment,
    DateTime CreatedAtUtc);
