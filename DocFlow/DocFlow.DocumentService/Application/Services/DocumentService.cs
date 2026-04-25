using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Repositories;

namespace DocFlow.DocumentService.Application.Services;

public sealed class DocumentService(IDocumentRepository repository) : IDocumentService
{
    public Task<List<Document>> GetAsync(Guid tenantId, CancellationToken cancellationToken) =>
        repository.GetByTenantAsync(tenantId, cancellationToken);

    public async Task<Document> CreateAsync(Guid tenantId, Guid userId, CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var document = new Document
        {
            TenantId = tenantId,
            OwnerUserId = userId,
            Title = request.Title,
            Category = request.Category,
            Department = request.Department,
            TagsCsv = request.TagsCsv,
            ConfidentialityLevel = request.ConfidentialityLevel,
            ExpiresAtUtc = request.ExpiresAtUtc,
            CurrentVersionNumber = 1
        };

        await repository.AddAsync(document, cancellationToken);
        var initialVersion = new DocumentVersion
        {
            DocumentId = document.Id,
            VersionNumber = 1,
            FileName = request.FileName,
            StoragePath = request.StoragePath,
            SizeBytes = request.SizeBytes,
            UploadedByUserId = userId
        };

        await repository.AddVersionAsync(initialVersion, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<Document?> UpdateAsync(Guid id, Guid tenantId, Guid userId, UpdateDocumentRequest request, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(id, tenantId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        document.Title = request.Title ?? document.Title;
        document.Category = request.Category ?? document.Category;
        document.Department = request.Department ?? document.Department;
        document.TagsCsv = request.TagsCsv ?? document.TagsCsv;
        document.ConfidentialityLevel = request.ConfidentialityLevel ?? document.ConfidentialityLevel;
        document.ExpiresAtUtc = request.ExpiresAtUtc ?? document.ExpiresAtUtc;

        if (!string.IsNullOrWhiteSpace(request.NewFileName) && !string.IsNullOrWhiteSpace(request.NewStoragePath))
        {
            document.CurrentVersionNumber++;
            var newVersion = new DocumentVersion
            {
                DocumentId = document.Id,
                VersionNumber = document.CurrentVersionNumber,
                FileName = request.NewFileName,
                StoragePath = request.NewStoragePath,
                SizeBytes = request.NewSizeBytes ?? 0,
                UploadedByUserId = userId
            };

            await repository.AddVersionAsync(newVersion, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<List<DocumentVersion>> GetVersionsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(id, tenantId, cancellationToken);
        return document is null ? [] : await repository.GetVersionsAsync(id, cancellationToken);
    }

    public async Task<bool> RestoreVersionAsync(Guid id, int versionNumber, Guid tenantId, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(id, tenantId, cancellationToken);
        var version = await repository.GetVersionAsync(id, versionNumber, cancellationToken);

        if (document is null || version is null)
        {
            return false;
        }

        document.CurrentVersionNumber = versionNumber;
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
