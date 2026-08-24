using DocFlow.DocumentService.Application.Abstractions;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;

public sealed class RestoreDocumentVersionCommandHandler(IDocumentRepository repository)
    : ICommandHandler<RestoreDocumentVersionCommand, bool>
{
    public async Task<bool> Handle(RestoreDocumentVersionCommand command, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (document is null) return false;

        var version = await repository.GetVersionAsync(command.Id, command.VersionNumber, cancellationToken);
        if (version is null) return false;

        document.RestoreVersion(version, command.UserId);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
