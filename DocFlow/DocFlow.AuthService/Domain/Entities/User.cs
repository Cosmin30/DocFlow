using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuthService.Domain.Entities;

public enum UserRole
{
    SuperAdmin,
    CompanyAdmin,
    Manager,
    Employee,
    ReadOnlyAuditor
}

public sealed class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "TenantId is required.")]
    public Guid TenantId { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [StringLength(256, ErrorMessage = "Email must be at most 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PasswordHash is required.")]
    [StringLength(500, ErrorMessage = "PasswordHash must be at most 500 characters.")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Role value is invalid.")]
    public UserRole Role { get; set; } = UserRole.Employee;

    [Required(ErrorMessage = "IsActive is required.")]
    public bool IsActive { get; set; } = true;

    [Range(0, int.MaxValue, ErrorMessage = "FailedLoginAttempts cannot be negative.")]
    public int FailedLoginAttempts { get; set; }

    public DateTime? LockedUntilUtc { get; set; }

    [Required(ErrorMessage = "CreatedAtUtc is required.")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
