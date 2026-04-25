using System.ComponentModel.DataAnnotations;

namespace DocFlow.DocumentService.Domain.Entities;

public sealed class DocumentVersion
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "DocumentId is required.")]
    public Guid DocumentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "VersionNumber must be at least 1.")]
    public int VersionNumber { get; set; }

    [Required(ErrorMessage = "FileName is required.")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters.")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "StoragePath is required.")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "StoragePath must be between 1 and 1000 characters.")]
    public string StoragePath { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "SizeBytes must be greater than 0.")]
    public long SizeBytes { get; set; }

    [Required(ErrorMessage = "UploadedByUserId is required.")]
    public Guid UploadedByUserId { get; set; }

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
