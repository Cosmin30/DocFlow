using DocFlow.DocumentService.Application.CQRS.Abstractions;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;

public sealed record RestoreDocumentVersionCommand(
    Guid Id,
    int VersionNumber,
    Guid TenantId,
    Guid UserId) : ICommand<bool>;
