using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;

public sealed class GetDocumentsQueryHandler(IDocumentRepository repository)
    : IRequestHandler<GetDocumentsQuery, List<Document>>
{
    public Task<List<Document>> Handle(GetDocumentsQuery query, CancellationToken cancellationToken) =>
        repository.GetByTenantAsync(query.TenantId, cancellationToken);
}
