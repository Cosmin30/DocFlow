using DocFlow.DocumentService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DocFlow.DocumentService.Application.Contracts;

public sealed record CreateDocumentRequest(
    [Required, StringLength(200, MinimumLength = 2)] string Title,
    [Required, StringLength(100, MinimumLength = 2)] string Category,
    [Required, StringLength(100, MinimumLength = 2)] string Department,
    [StringLength(500)] string TagsCsv,
    ConfidentialityLevel ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    [Required, StringLength(255, MinimumLength = 1)] string FileName,
    [Required, StringLength(1000, MinimumLength = 1)] string StoragePath,
    [Range(1, long.MaxValue)]
    long SizeBytes);

public sealed record UpdateDocumentRequest(
    [StringLength(200, MinimumLength = 2)]
    string? Title,
    [StringLength(100, MinimumLength = 2)]
    string? Category,
    [StringLength(100, MinimumLength = 2)]
    string? Department,
    [StringLength(500)]
    string? TagsCsv,
    ConfidentialityLevel? ConfidentialityLevel,
    DateTime? ExpiresAtUtc,
    [StringLength(255, MinimumLength = 1)]
    string? NewFileName,
    [StringLength(1000, MinimumLength = 1)]
    string? NewStoragePath,
    [Range(1, long.MaxValue)]
    long? NewSizeBytes);
