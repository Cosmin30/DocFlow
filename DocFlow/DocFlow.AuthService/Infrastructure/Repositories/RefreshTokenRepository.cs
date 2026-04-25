using DocFlow.AuthService.Domain.Entities;
using DocFlow.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuthService.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AuthDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsRevoked, cancellationToken);

    public Task<List<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
