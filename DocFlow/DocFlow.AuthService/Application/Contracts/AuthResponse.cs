namespace DocFlow.AuthService.Application.Contracts;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    Guid TenantId,
    string Role);
