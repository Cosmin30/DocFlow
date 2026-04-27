using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Infrastructure.Repositories;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;

public sealed class RestoreDocumentVersionCommandHandler(IDocumentRepository repository)
    : ICommandHandler<RestoreDocumentVersionCommand, bool>
{
    public async Task<bool> Handle(RestoreDocumentVersionCommand command, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(command.Id, command.TenantId, cancellationToken);
        var version = await repository.GetVersionAsync(command.Id, command.VersionNumber, cancellationToken);

        if (document is null || version is null)
        {
            return false;
        }

        document.CurrentVersionNumber = command.VersionNumber;
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
