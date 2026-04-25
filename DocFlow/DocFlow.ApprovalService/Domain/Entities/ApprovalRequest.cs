using System.ComponentModel.DataAnnotations;

namespace DocFlow.ApprovalService.Domain.Entities;

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}

public sealed class ApprovalRequest
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "TenantId is required.")]
    public Guid TenantId { get; set; }

    [Required(ErrorMessage = "DocumentId is required.")]
    public Guid DocumentId { get; set; }

    [Required(ErrorMessage = "RequestedByUserId is required.")]
    public Guid RequestedByUserId { get; set; }

    [Required(ErrorMessage = "AssignedToUserId is required.")]
    public Guid AssignedToUserId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [EnumDataType(typeof(ApprovalStatus), ErrorMessage = "Status value is invalid.")]
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    [StringLength(1000, ErrorMessage = "Comment must be at most 1000 characters.")]
    public string? Comment { get; set; }

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAtUtc { get; set; }
}
