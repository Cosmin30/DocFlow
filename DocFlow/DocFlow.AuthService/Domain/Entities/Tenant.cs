using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuthService.Domain.Entities;

public sealed class Tenant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Slug must be between 2 and 100 characters.")]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Slug can contain only lowercase letters, digits, and '-'.")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
