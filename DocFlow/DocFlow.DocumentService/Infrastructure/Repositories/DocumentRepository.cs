using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.DocumentService.Infrastructure.Repositories;

public sealed class DocumentRepository(DocumentDbContext dbContext) : IDocumentRepository
{
    public Task<List<Document>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken) =>
        dbContext.Documents
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Versions)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<Document?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken) =>
        dbContext.Documents
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public async Task AddAsync(Document document, CancellationToken cancellationToken)
    {
        await dbContext.Documents.AddAsync(document, cancellationToken);
    }

    public Task<List<DocumentVersion>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken) =>
        dbContext.DocumentVersions
            .Where(x => x.DocumentId == documentId)
            .OrderByDescending(x => x.VersionNumber)
            .ToListAsync(cancellationToken);

    public Task<DocumentVersion?> GetVersionAsync(Guid documentId, int versionNumber, CancellationToken cancellationToken) =>
        dbContext.DocumentVersions
            .FirstOrDefaultAsync(x => x.DocumentId == documentId && x.VersionNumber == versionNumber, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
