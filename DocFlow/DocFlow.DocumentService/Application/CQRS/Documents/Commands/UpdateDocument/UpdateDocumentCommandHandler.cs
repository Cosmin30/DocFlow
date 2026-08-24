using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;

public sealed class UpdateDocumentCommandHandler(IDocumentRepository repository)
    : IRequestHandler<UpdateDocumentCommand, Document?>
{
    public async Task<Document?> Handle(UpdateDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (document is null) return null;

        document.Update(
            command.UserId,
            command.Request.Title,
            command.Request.Category,
            command.Request.Department,
            command.Request.TagsCsv,
            command.Request.ConfidentialityLevel,
            command.Request.ExpiresAtUtc);

        if (!string.IsNullOrWhiteSpace(command.Request.NewFileName) && !string.IsNullOrWhiteSpace(command.Request.NewStoragePath))
        {
            document.AddVersion(
                command.Request.NewFileName,
                command.Request.NewStoragePath,
                command.Request.NewSizeBytes ?? 0,
                command.UserId);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return document;
    }
}
