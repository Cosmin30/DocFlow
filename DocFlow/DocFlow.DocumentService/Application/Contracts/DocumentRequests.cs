using DocFlow.DocumentService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DocFlow.DocumentService.Application.Contracts;

public sealed record CreateDocumentRequest(
    [property: Required, StringLength(200, MinimumLength = 2)] string Title,
    [property: Required, StringLength(100, MinimumLength = 2)] string Category,
    [property: Required, StringLength(100, MinimumLength = 2)] string Department,
    [property: StringLength(500)] string TagsCsv,
    ConfidentialityLevel ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    [property: Required, StringLength(255, MinimumLength = 1)] string FileName,
    [property: Required, StringLength(1000, MinimumLength = 1)] string StoragePath,
    [property: Range(1, long.MaxValue)]
    long SizeBytes);

public sealed record UpdateDocumentRequest(
    [property: StringLength(200, MinimumLength = 2)]
    string? Title,
    [property: StringLength(100, MinimumLength = 2)]
    string? Category,
    [property: StringLength(100, MinimumLength = 2)]
    string? Department,
    [property: StringLength(500)]
    string? TagsCsv,
    ConfidentialityLevel? ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    [property: StringLength(255, MinimumLength = 1)]
    string? NewFileName,
    [property: StringLength(1000, MinimumLength = 1)]
    string? NewStoragePath,
    [property: Range(1, long.MaxValue)]
    long? NewSizeBytes);
