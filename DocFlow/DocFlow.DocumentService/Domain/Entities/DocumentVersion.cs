using DocFlow.BuildingBlocks.Domain;

namespace DocFlow.DocumentService.Domain.Entities;

public sealed class DocumentVersion : Entity
{
    public Guid DocumentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private DocumentVersion() { }

    public DocumentVersion(
        Guid documentId,
        int versionNumber,
        string fileName,
        string storagePath,
        long sizeBytes,
        Guid uploadedByUserId)
    {
        Guard.NotEmpty(documentId, nameof(documentId));
        Guard.NotNegativeOrZero(versionNumber, nameof(versionNumber));
        Guard.NotNullOrWhiteSpace(fileName, nameof(fileName));
        Guard.MaxLength(fileName, 255, nameof(fileName));
        Guard.NotNullOrWhiteSpace(storagePath, nameof(storagePath));
        Guard.MaxLength(storagePath, 1000, nameof(storagePath));
        Guard.NotNegative(sizeBytes, nameof(sizeBytes));
        Guard.NotEmpty(uploadedByUserId, nameof(uploadedByUserId));

        DocumentId = documentId;
        VersionNumber = versionNumber;
        FileName = fileName;
        StoragePath = storagePath;
        SizeBytes = sizeBytes;
        UploadedByUserId = uploadedByUserId;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
