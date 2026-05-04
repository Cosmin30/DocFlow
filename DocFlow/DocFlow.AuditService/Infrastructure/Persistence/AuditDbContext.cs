using DocFlow.AuditService.Domain.Entities;
using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.AuditService.Infrastructure.Persistence;

public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>()
            .HasIndex(x => new { x.TenantId, x.CreatedAtUtc });
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
