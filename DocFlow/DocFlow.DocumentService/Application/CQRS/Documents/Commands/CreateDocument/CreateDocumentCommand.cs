using DocFlow.DocumentService.Application.Contracts;
using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;

public sealed record CreateDocumentCommand(
    Guid TenantId,
    Guid UserId,
    CreateDocumentRequest Request) : ICommand<Document>;
