using DocFlow.BuildingBlocks.Domain;

namespace DocFlow.DocumentService.Domain.Entities;

public enum ConfidentialityLevel
{
    Public,
    Internal,
    Confidential,
    Strict
}

public sealed class Document : AggregateRoot
{
    private readonly List<DocumentVersion> _versions = [];

    public Guid TenantId { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string TagsCsv { get; private set; } = string.Empty;
    public ConfidentialityLevel ConfidentialityLevel { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public int CurrentVersionNumber { get; private set; } = 1;
    public string CurrentFileName { get; private set; } = string.Empty;
    public string CurrentStoragePath { get; private set; } = string.Empty;
    public long CurrentSizeBytes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public IReadOnlyList<DocumentVersion> Versions => _versions.AsReadOnly();

    private Document() { }

    public static Document Create(
        Guid tenantId,
        Guid ownerUserId,
        string title,
        string category,
        string department,
        string tagsCsv,
        ConfidentialityLevel confidentialityLevel,
        DateTime? expiresAtUtc,
        string fileName,
        string storagePath,
        long sizeBytes)
    {
        Guard.NotEmpty(tenantId, nameof(tenantId));
        Guard.NotEmpty(ownerUserId, nameof(ownerUserId));
        Guard.NotNullOrWhiteSpace(title, nameof(title));
        Guard.MaxLength(title, 200, nameof(title));
        Guard.MinLength(title, 2, nameof(title));
        Guard.NotNullOrWhiteSpace(category, nameof(category));
        Guard.MaxLength(category, 100, nameof(category));
        Guard.NotNullOrWhiteSpace(department, nameof(department));
        Guard.MaxLength(department, 100, nameof(department));
        Guard.MaxLength(tagsCsv, 500, nameof(tagsCsv));
        Guard.EnumIsValid(confidentialityLevel, nameof(confidentialityLevel));
        Guard.NotNullOrWhiteSpace(fileName, nameof(fileName));
        Guard.NotNullOrWhiteSpace(storagePath, nameof(storagePath));
        Guard.NotNegative(sizeBytes, nameof(sizeBytes));

        var document = new Document
        {
            TenantId = tenantId,
            OwnerUserId = ownerUserId,
            Title = title.Trim(),
            Category = category.Trim(),
            Department = department.Trim(),
            TagsCsv = tagsCsv,
            ConfidentialityLevel = confidentialityLevel,
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        document.AddVersion(1, fileName, storagePath, sizeBytes, ownerUserId);

        document.RaiseDomainEvent(new DocumentCreatedDomainEvent(document.Id, tenantId, ownerUserId, title));

        return document;
    }

    public void Update(
        Guid userId,
        string? title = null,
        string? category = null,
        string? department = null,
        string? tagsCsv = null,
        ConfidentialityLevel? confidentialityLevel = null,
        DateTime? expiresAtUtc = null)
    {
        if (OwnerUserId != userId)
            throw new InvalidOperationException("Only the document owner can update it.");

        if (title is not null)
        {
            Guard.NotNullOrWhiteSpace(title, nameof(title));
            Guard.MaxLength(title, 200, nameof(title));
            Guard.MinLength(title, 2, nameof(title));
            Title = title.Trim();
        }

        if (category is not null)
        {
            Guard.NotNullOrWhiteSpace(category, nameof(category));
            Guard.MaxLength(category, 100, nameof(category));
            Category = category.Trim();
        }

        if (department is not null)
        {
            Guard.NotNullOrWhiteSpace(department, nameof(department));
            Guard.MaxLength(department, 100, nameof(department));
            Department = department.Trim();
        }

        if (tagsCsv is not null) TagsCsv = tagsCsv;
        if (confidentialityLevel.HasValue)
        {
            Guard.EnumIsValid(confidentialityLevel.Value, nameof(confidentialityLevel));
            ConfidentialityLevel = confidentialityLevel.Value;
        }

        if (expiresAtUtc.HasValue) ExpiresAtUtc = expiresAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public DocumentVersion AddVersion(string fileName, string storagePath, long sizeBytes, Guid uploadedByUserId)
    {
        Guard.NotNullOrWhiteSpace(fileName, nameof(fileName));
        Guard.NotNullOrWhiteSpace(storagePath, nameof(storagePath));
        Guard.NotNegative(sizeBytes, nameof(sizeBytes));
        Guard.NotEmpty(uploadedByUserId, nameof(uploadedByUserId));

        CurrentVersionNumber++;
        var version = new DocumentVersion(
            Id,
            CurrentVersionNumber,
            fileName,
            storagePath,
            sizeBytes,
            uploadedByUserId);

        _versions.Add(version);

        CurrentFileName = fileName;
        CurrentStoragePath = storagePath;
        CurrentSizeBytes = sizeBytes;
        UpdatedAtUtc = DateTime.UtcNow;

        return version;
    }

    public void RestoreVersion(DocumentVersion version, Guid userId)
    {
        if (OwnerUserId != userId)
            throw new InvalidOperationException("Only the document owner can restore versions.");

        if (version is null)
            throw new ArgumentNullException(nameof(version));

        if (version.DocumentId != Id)
            throw new InvalidOperationException("Version does not belong to this document.");

        CurrentVersionNumber = version.VersionNumber;
        CurrentFileName = version.FileName;
        CurrentStoragePath = version.StoragePath;
        CurrentSizeBytes = version.SizeBytes;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Delete(Guid userId)
    {
        if (OwnerUserId != userId)
            throw new InvalidOperationException("Only the document owner can delete it.");

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DocumentDeletedDomainEvent(Id, TenantId, userId));
    }
}

public record DocumentCreatedDomainEvent(Guid DocumentId, Guid TenantId, Guid OwnerUserId, string Title) : DomainEvent;
public record DocumentDeletedDomainEvent(Guid DocumentId, Guid TenantId, Guid OwnerUserId) : DomainEvent;
