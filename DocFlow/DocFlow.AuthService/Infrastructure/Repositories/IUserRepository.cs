using DocFlow.AuthService.Domain.Entities;

namespace DocFlow.AuthService.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<Tenant?> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> TenantSlugExistsAsync(string slug, CancellationToken cancellationToken);
    Task AddTenantAsync(Tenant tenant, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
