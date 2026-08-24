using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.BuildingBlocks.Messaging.Outbox;

public interface IOutboxStore
{
    Task AddAsync<T>(T integrationEvent, string topicName, CancellationToken cancellationToken = default);
}

public sealed class OutboxStore<TDbContext>(TDbContext dbContext) : IOutboxStore
    where TDbContext : DbContext
{
    public async Task AddAsync<T>(T integrationEvent, string topicName, CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage
        {
            EventType = typeof(T).Name,
            Payload = JsonSerializer.Serialize(integrationEvent),
            TopicName = topicName
        };

        await dbContext.Set<OutboxMessage>().AddAsync(message, cancellationToken);
    }
}
