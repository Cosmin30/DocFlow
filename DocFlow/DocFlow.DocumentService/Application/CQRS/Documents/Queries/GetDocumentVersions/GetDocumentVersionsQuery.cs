using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;

public sealed record GetDocumentVersionsQuery(Guid DocumentId, Guid TenantId) : IQuery<List<DocumentVersion>>;
