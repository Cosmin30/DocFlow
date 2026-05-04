using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Repositories;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;

public sealed class UpdateDocumentCommandHandler(IDocumentRepository repository)
    : ICommandHandler<UpdateDocumentCommand, Document?>
{
    public async Task<Document?> Handle(UpdateDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        if (document.OwnerUserId != command.UserId)
        {
            return null;
        }

        document.Title = command.Request.Title ?? document.Title;
        document.Category = command.Request.Category ?? document.Category;
        document.Department = command.Request.Department ?? document.Department;
        document.TagsCsv = command.Request.TagsCsv ?? document.TagsCsv;
        document.ConfidentialityLevel = command.Request.ConfidentialityLevel ?? document.ConfidentialityLevel;
        document.ExpiresAtUtc = command.Request.ExpiresAtUtc ?? document.ExpiresAtUtc;

        if (!string.IsNullOrWhiteSpace(command.Request.NewFileName) && !string.IsNullOrWhiteSpace(command.Request.NewStoragePath))
        {
            document.CurrentVersionNumber++;
            var newVersion = new DocumentVersion
            {
                DocumentId = document.Id,
                VersionNumber = document.CurrentVersionNumber,
                FileName = command.Request.NewFileName,
                StoragePath = command.Request.NewStoragePath,
                SizeBytes = command.Request.NewSizeBytes ?? 0,
                UploadedByUserId = command.UserId
            };

            await repository.AddVersionAsync(newVersion, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return document;
    }
}
