using DocFlow.AuthService.Domain.Entities;

namespace DocFlow.AuthService.Infrastructure.Security;

public sealed record TokenPair(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

public interface ITokenService
{
    TokenPair Create(User user);
    string Hash(string refreshToken);
}
