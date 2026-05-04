using DocFlow.ApprovalService.Domain.Entities;
using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.ApprovalService.Infrastructure.Persistence;

public sealed class ApprovalDbContext(DbContextOptions<ApprovalDbContext> options) : DbContext(options)
{
    public DbSet<ApprovalRequest> Approvals => Set<ApprovalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRequest>()
            .HasIndex(x => new { x.TenantId, x.Status });
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
