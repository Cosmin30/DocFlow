using DocFlow.AuthService.Application.Contracts;
using DocFlow.AuthService.Domain.Entities;
using DocFlow.AuthService.Infrastructure.Repositories;
using DocFlow.AuthService.Infrastructure.Security;

namespace DocFlow.AuthService.Application.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var slug = request.TenantSlug.Trim().ToLowerInvariant();
        if (await userRepository.TenantSlugExistsAsync(slug, cancellationToken))
        {
            return AuthResult.Fail("Tenant slug already exists.");
        }

        var tenant = new Tenant
        {
            Name = request.TenantName.Trim(),
            Slug = slug
        };

        var user = new User
        {
            TenantId = tenant.Id,
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = request.Role
        };

        await userRepository.AddTenantAsync(tenant, cancellationToken);
        await userRepository.AddUserAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok(new AuthResponse(string.Empty, string.Empty, DateTime.UtcNow, user.Id, tenant.Id, user.Role.ToString()));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, string ipAddress, string device, CancellationToken cancellationToken)
    {
        var tenant = await userRepository.GetTenantBySlugAsync(request.TenantSlug.Trim().ToLowerInvariant(), cancellationToken);
        if (tenant is null)
        {
            return AuthResult.Fail("Invalid credentials.");
        }

        var user = await userRepository.GetByEmailAsync(tenant.Id, request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            return AuthResult.Fail("Invalid credentials.");
        }

        if (user.LockedUntilUtc.HasValue && user.LockedUntilUtc > DateTime.UtcNow)
        {
            return AuthResult.Fail("Account temporarily locked.");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntilUtc = DateTime.UtcNow.AddMinutes(15);
                user.FailedLoginAttempts = 0;
            }

            await userRepository.SaveChangesAsync(cancellationToken);
            return AuthResult.Fail("Invalid credentials.");
        }

        user.FailedLoginAttempts = 0;
        user.LockedUntilUtc = null;
        await userRepository.SaveChangesAsync(cancellationToken);

        var pair = tokenService.Create(user);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.Hash(pair.RefreshToken),
            Device = string.IsNullOrWhiteSpace(device) ? "unknown" : device,
            IpAddress = ipAddress,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok(new AuthResponse(pair.AccessToken, pair.RefreshToken, pair.ExpiresAtUtc, user.Id, user.TenantId, user.Role.ToString()));
    }

    public async Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        var hashed = tokenService.Hash(request.RefreshToken);
        var token = await refreshTokenRepository.GetByHashAsync(hashed, cancellationToken);
        if (token is null || token.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return AuthResult.Fail("Invalid refresh token.");
        }

        var user = await userRepository.GetByIdAsync(token.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return AuthResult.Fail("Invalid refresh token.");
        }

        token.IsRevoked = true;
        var pair = tokenService.Create(user);

        var newToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.Hash(pair.RefreshToken),
            Device = token.Device,
            IpAddress = token.IpAddress,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.AddAsync(newToken, cancellationToken);

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok(new AuthResponse(pair.AccessToken, pair.RefreshToken, pair.ExpiresAtUtc, user.Id, user.TenantId, user.Role.ToString()));
    }

    public async Task LogoutAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await refreshTokenRepository.GetActiveByUserAsync(userId, cancellationToken);
        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
        }

        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}
