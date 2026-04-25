using DocFlow.AuthService.Domain.Entities;
using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuthService.Infrastructure.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>().HasIndex(x => x.Slug).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        modelBuilder.Entity<RefreshToken>().HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
    }

    public override int SaveChanges()
    {
        this.ValidateTrackedEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ValidateTrackedEntities();
        return base.SaveChangesAsync(cancellationToken);
    }
}
