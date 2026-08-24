using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;

public sealed record GetDocumentsQuery(Guid TenantId) : IRequest<List<Document>>;
