namespace DocFlow.BuildingBlocks.Messaging.Events;

public sealed record ApprovalDecidedIntegrationEvent(
    Guid TenantId,
    Guid ApprovalId,
    Guid DocumentId,
    Guid DecidedByUserId,
    bool Approved,
    string? Comment,
    DateTime ResolvedAtUtc);
