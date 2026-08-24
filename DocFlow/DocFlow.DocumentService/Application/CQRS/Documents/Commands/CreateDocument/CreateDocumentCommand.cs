using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;

public sealed record CreateDocumentCommand(
    Guid TenantId,
    Guid UserId,
    Contracts.CreateDocumentRequest Request) : IRequest<Document>;
