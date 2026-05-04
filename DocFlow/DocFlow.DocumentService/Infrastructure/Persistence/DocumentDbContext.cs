using DocFlow.DocumentService.Domain.Entities;
using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.DocumentService.Infrastructure.Persistence;

public sealed class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasIndex(x => new { x.TenantId, x.Department, x.Category });

        modelBuilder.Entity<DocumentVersion>()
            .HasIndex(x => new { x.DocumentId, x.VersionNumber })
            .IsUnique();
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
