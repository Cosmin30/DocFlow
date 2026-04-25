using DocFlow.AuthService.Application.Contracts;

namespace DocFlow.AuthService.Application.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(LoginRequest request, string ipAddress, string device, CancellationToken cancellationToken);
    Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
    Task LogoutAllAsync(Guid userId, CancellationToken cancellationToken);
}
