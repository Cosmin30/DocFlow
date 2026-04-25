using DocFlow.AuditService.Domain.Entities;
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
}
