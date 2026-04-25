using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Infrastructure.Repositories;

public interface IDocumentRepository
{
    Task<List<Document>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<Document?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task AddAsync(Document document, CancellationToken cancellationToken);
    Task AddVersionAsync(DocumentVersion version, CancellationToken cancellationToken);
    Task<List<DocumentVersion>> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken);
    Task<DocumentVersion?> GetVersionAsync(Guid documentId, int versionNumber, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
