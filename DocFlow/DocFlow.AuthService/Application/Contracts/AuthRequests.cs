using DocFlow.AuthService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuthService.Application.Contracts;

public sealed record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 2)] string TenantName,
    [Required, StringLength(100, MinimumLength = 2), RegularExpression("^[a-z0-9-]+$")] string TenantSlug,
    [Required, StringLength(150, MinimumLength = 2)] string FullName,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    UserRole Role);

public sealed record LoginRequest(
    [Required, StringLength(100, MinimumLength = 2), RegularExpression("^[a-z0-9-]+$")] string TenantSlug,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    [StringLength(256)] string? Device);

public sealed record RefreshRequest(
    [Required, StringLength(512, MinimumLength = 20)] string RefreshToken);
