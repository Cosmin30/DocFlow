using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuditService.Domain.Entities;

public sealed class AuditLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "TenantId is required.")]
    public Guid TenantId { get; set; }

    public Guid? UserId { get; set; }

    [Required(ErrorMessage = "Action is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Action must be between 2 and 100 characters.")]
    public string Action { get; set; } = string.Empty;

    [Required(ErrorMessage = "EntityType is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "EntityType must be between 2 and 100 characters.")]
    public string EntityType { get; set; } = string.Empty;

    [Required(ErrorMessage = "EntityId is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "EntityId must be between 1 and 100 characters.")]
    public string EntityId { get; set; } = string.Empty;

    [StringLength(4000, ErrorMessage = "MetadataJson must be at most 4000 characters.")]
    public string? MetadataJson { get; set; }

    [Required(ErrorMessage = "IpAddress is required.")]
    [StringLength(64, ErrorMessage = "IpAddress must be at most 64 characters.")]
    public string IpAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device is required.")]
    [StringLength(256, ErrorMessage = "Device must be at most 256 characters.")]
    public string Device { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
