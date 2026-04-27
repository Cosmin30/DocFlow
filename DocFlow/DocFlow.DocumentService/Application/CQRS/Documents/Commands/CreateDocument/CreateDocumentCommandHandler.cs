using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Repositories;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;

public sealed class CreateDocumentCommandHandler(IDocumentRepository repository)
    : ICommandHandler<CreateDocumentCommand, Document>
{
    public async Task<Document> Handle(CreateDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = new Document
        {
            TenantId = command.TenantId,
            OwnerUserId = command.UserId,
            Title = command.Request.Title,
            Category = command.Request.Category,
            Department = command.Request.Department,
            TagsCsv = command.Request.TagsCsv,
            ConfidentialityLevel = command.Request.ConfidentialityLevel,
            ExpiresAtUtc = command.Request.ExpiresAtUtc,
            CurrentVersionNumber = 1
        };

        await repository.AddAsync(document, cancellationToken);

        var initialVersion = new DocumentVersion
        {
            DocumentId = document.Id,
            VersionNumber = 1,
            FileName = command.Request.FileName,
            StoragePath = command.Request.StoragePath,
            SizeBytes = command.Request.SizeBytes,
            UploadedByUserId = command.UserId
        };

        await repository.AddVersionAsync(initialVersion, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return document;
    }
}
