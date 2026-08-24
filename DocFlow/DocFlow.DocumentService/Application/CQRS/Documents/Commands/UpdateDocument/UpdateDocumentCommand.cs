using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;

public sealed record UpdateDocumentCommand(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    UpdateDocumentRequest Request) : IRequest<Document?>;
