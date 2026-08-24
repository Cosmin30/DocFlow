using DocFlow.DocumentService.Application.Contracts;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;

public sealed record UpdateDocumentCommand(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    UpdateDocumentRequest Request) : IRequest<Document?>;
