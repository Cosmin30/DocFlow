using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.BuildingBlocks.Messaging.Outbox;
using MediatR;

namespace DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;

public sealed class CreateDocumentCommandHandler(IDocumentRepository repository, IOutboxStore outboxStore)
    : IRequestHandler<CreateDocumentCommand, Document>
{
    public async Task<Document> Handle(CreateDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = Document.Create(
            command.TenantId,
            command.UserId,
            command.Request.Title,
            command.Request.Category,
            command.Request.Department,
            command.Request.TagsCsv,
            command.Request.ConfidentialityLevel,
            command.Request.ExpiresAtUtc,
            command.Request.FileName,
            command.Request.StoragePath,
            command.Request.SizeBytes);

        await repository.AddAsync(document, cancellationToken);

        foreach (var domainEvent in document.DomainEvents)
        {
            if (domainEvent is DocumentCreatedDomainEvent)
            {
                await outboxStore.AddAsync(
                    new BuildingBlocks.Messaging.Events.DocumentCreatedIntegrationEvent(
                        command.TenantId,
                        document.Id,
                        command.UserId,
                        document.Title,
                        DateTime.UtcNow),
                    topicName: "docflow.document.created",
                    cancellationToken);

                await outboxStore.AddAsync(
                    new BuildingBlocks.Messaging.Events.NotificationIntegrationEvent(
                        command.TenantId,
                        UserId: command.UserId,
                        Title: "Document created",
                        Message: $"Document '{document.Title}' was created.",
                        CreatedAtUtc: DateTime.UtcNow),
                    topicName: "docflow.notifications",
                    cancellationToken);
            }
        }

        document.ClearDomainEvents();
        await repository.SaveChangesAsync(cancellationToken);

        return document;
    }
}
