using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Repositories;
using DocFlow.BuildingBlocks.Messaging;
using DocFlow.BuildingBlocks.Messaging.Events;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;

public sealed class CreateDocumentCommandHandler(IDocumentRepository repository, IEventBus eventBus)
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

        await eventBus.PublishAsync(
            new DocumentCreatedIntegrationEvent(
                command.TenantId,
                document.Id,
                command.UserId,
                document.Title,
                DateTime.UtcNow),
            topicName: "docflow.document.created",
            cancellationToken);

        await eventBus.PublishAsync(
            new NotificationIntegrationEvent(
                command.TenantId,
                UserId: command.UserId,
                Title: "Document created",
                Message: $"Document '{document.Title}' was created.",
                CreatedAtUtc: DateTime.UtcNow),
            topicName: "docflow.notifications",
            cancellationToken);

        return document;
    }
}
