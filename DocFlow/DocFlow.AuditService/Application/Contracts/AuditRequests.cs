namespace DocFlow.AuditService.Application.Contracts;

using System.ComponentModel.DataAnnotations;

public sealed record WriteAuditRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Action,
    [Required, StringLength(100, MinimumLength = 2)] string EntityType,
    [Required, StringLength(100, MinimumLength = 1)] string EntityId,
    [StringLength(4000)] string? MetadataJson);
