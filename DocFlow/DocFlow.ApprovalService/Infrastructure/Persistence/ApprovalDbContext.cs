using DocFlow.ApprovalService.Domain.Entities;
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
}
