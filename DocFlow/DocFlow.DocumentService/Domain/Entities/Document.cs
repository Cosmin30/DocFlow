using System.ComponentModel.DataAnnotations;

namespace DocFlow.DocumentService.Domain.Entities;

public enum ConfidentialityLevel
{
    Public,
    Internal,
    Confidential,
    Strict
}

public sealed class Document
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "TenantId is required.")]
    public Guid TenantId { get; set; }

    [Required(ErrorMessage = "OwnerUserId is required.")]
    public Guid OwnerUserId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 100 characters.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "TagsCsv must be at most 500 characters.")]
    public string TagsCsv { get; set; } = string.Empty;

    [Required(ErrorMessage = "ConfidentialityLevel is required.")]
    [EnumDataType(typeof(ConfidentialityLevel), ErrorMessage = "ConfidentialityLevel value is invalid.")]
    public ConfidentialityLevel ConfidentialityLevel { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CurrentVersionNumber must be at least 1.")]
    public int CurrentVersionNumber { get; set; } = 1;

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
