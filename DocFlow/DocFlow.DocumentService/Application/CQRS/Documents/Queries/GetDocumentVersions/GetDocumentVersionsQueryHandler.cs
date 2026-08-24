using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;

public sealed class GetDocumentVersionsQueryHandler(IDocumentRepository repository)
    : IRequestHandler<GetDocumentVersionsQuery, List<DocumentVersion>>
{
    public async Task<List<DocumentVersion>> Handle(GetDocumentVersionsQuery query, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(query.DocumentId, query.TenantId, cancellationToken);
        return document is null ? [] : await repository.GetVersionsAsync(query.DocumentId, cancellationToken);
    }
}
