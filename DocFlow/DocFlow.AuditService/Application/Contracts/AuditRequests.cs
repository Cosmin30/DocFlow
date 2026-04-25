namespace DocFlow.AuditService.Application.Contracts;

using System.ComponentModel.DataAnnotations;

public sealed record WriteAuditRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string Action,
    [property: Required, StringLength(100, MinimumLength = 2)] string EntityType,
    [property: Required, StringLength(100, MinimumLength = 1)] string EntityId,
    [property: StringLength(4000)] string? MetadataJson,
    [property: StringLength(64)] string? IpAddress,
    [property: StringLength(256)] string? Device);
