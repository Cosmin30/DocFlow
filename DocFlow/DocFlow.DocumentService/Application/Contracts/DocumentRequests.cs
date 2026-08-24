using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.Contracts;

public sealed record CreateDocumentRequest(
    string Title,
    string Category,
    string Department,
    string TagsCsv,
    ConfidentialityLevel ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    string FileName,
    string StoragePath,
    long SizeBytes);

public sealed record UpdateDocumentRequest(
    string? Title,
    string? Category,
    string? Department,
    string? TagsCsv,
    ConfidentialityLevel? ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    string? NewFileName,
    string? NewStoragePath,
    long? NewSizeBytes);
