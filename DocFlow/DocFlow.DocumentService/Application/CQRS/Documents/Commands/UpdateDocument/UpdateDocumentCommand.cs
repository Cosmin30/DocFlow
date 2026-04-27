using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;

public sealed record UpdateDocumentCommand(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    UpdateDocumentRequest Request) : ICommand<Document?>;
