using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;

public sealed record GetDocumentsQuery(Guid TenantId) : IQuery<List<Document>>;
