using DocFlow.AuthService.Domain.Entities;
using DocFlow.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuthService.Infrastructure.Repositories;

public sealed class UserRepository(AuthDbContext dbContext) : IUserRepository
{
    public Task<Tenant?> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken) =>
        dbContext.Tenants.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

    public Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task<bool> TenantSlugExistsAsync(string slug, CancellationToken cancellationToken) =>
        dbContext.Tenants.AnyAsync(x => x.Slug == slug, cancellationToken);

    public async Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        await dbContext.Tenants.AddAsync(tenant, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
