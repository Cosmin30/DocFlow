using DocFlow.DocumentService.Domain.Entities;
using DocFlow.BuildingBlocks.Domain;
using DocFlow.BuildingBlocks.Messaging.Outbox;
using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.DocumentService.Infrastructure.Persistence;

public sealed class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.TenantId, x.Department, x.Category });
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Ignore(e => e.DomainEvents);

            entity.Navigation(e => e.Versions).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<DocumentVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(x => new { x.DocumentId, x.VersionNumber }).IsUnique();
        });

        modelBuilder.AddOutboxModelCreating();
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
