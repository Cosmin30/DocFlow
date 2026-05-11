namespace DocFlow.BuildingBlocks.Messaging.Events;

public sealed record DocumentCreatedIntegrationEvent(
    Guid TenantId,
    Guid DocumentId,
    Guid OwnerUserId,
    string Title,
    DateTime CreatedAtUtc);
