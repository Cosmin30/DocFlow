using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.Services;

public interface IDocumentService
{
    Task<List<Document>> GetAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<Document> CreateAsync(Guid tenantId, Guid userId, CreateDocumentRequest request, CancellationToken cancellationToken);
    Task<Document?> UpdateAsync(Guid id, Guid tenantId, Guid userId, UpdateDocumentRequest request, CancellationToken cancellationToken);
    Task<List<DocumentVersion>> GetVersionsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task<bool> RestoreVersionAsync(Guid id, int versionNumber, Guid tenantId, CancellationToken cancellationToken);
}
