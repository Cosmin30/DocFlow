using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuthService.Domain.Entities;

public sealed class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "UserId is required.")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "TokenHash is required.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "TokenHash must be between 10 and 500 characters.")]
    public string TokenHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device is required.")]
    [StringLength(256, ErrorMessage = "Device must be at most 256 characters.")]
    public string Device { get; set; } = string.Empty;

    [Required(ErrorMessage = "IpAddress is required.")]
    [StringLength(64, ErrorMessage = "IpAddress must be at most 64 characters.")]
    public string IpAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "IsRevoked is required.")]
    public bool IsRevoked { get; set; }

    [Required(ErrorMessage = "ExpiresAtUtc is required.")]
    public DateTime ExpiresAtUtc { get; set; }

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
