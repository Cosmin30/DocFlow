namespace DocFlow.BuildingBlocks.Messaging.Events;

public sealed record NotificationIntegrationEvent(
    Guid TenantId,
    Guid? UserId,
    string Title,
    string Message,
    DateTime CreatedAtUtc);
