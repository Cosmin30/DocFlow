using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;

public sealed record RestoreDocumentVersionCommand(
    Guid Id,
    int VersionNumber,
    Guid TenantId,
    Guid UserId) : IRequest<bool>;
