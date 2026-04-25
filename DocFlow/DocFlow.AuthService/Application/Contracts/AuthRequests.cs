using DocFlow.AuthService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DocFlow.AuthService.Application.Contracts;

public sealed record RegisterRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string TenantName,
    [property: Required, StringLength(100, MinimumLength = 2), RegularExpression("^[a-z0-9-]+$")] string TenantSlug,
    [property: Required, StringLength(150, MinimumLength = 2)] string FullName,
    [property: Required, EmailAddress, StringLength(256)] string Email,
    [property: Required, StringLength(128, MinimumLength = 8)] string Password,
    UserRole Role);

public sealed record LoginRequest(
    [property: Required, StringLength(100, MinimumLength = 2), RegularExpression("^[a-z0-9-]+$")] string TenantSlug,
    [property: Required, EmailAddress, StringLength(256)] string Email,
    [property: Required, StringLength(128, MinimumLength = 8)] string Password,
    [property: StringLength(256)] string? Device);

public sealed record RefreshRequest(
    [property: Required, StringLength(512, MinimumLength = 20)] string RefreshToken);
