using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;

public sealed record GetDocumentVersionsQuery(Guid DocumentId, Guid TenantId) : IRequest<List<DocumentVersion>>;
