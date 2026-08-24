using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocFlow.BuildingBlocks.Messaging.Outbox;

public sealed class OutboxProcessor<TDbContext>(
    IServiceProvider serviceProvider,
    IEventBus eventBus,
    ILogger<OutboxProcessor<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedAtUtc == null)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType($"DocFlow.BuildingBlocks.Messaging.Events.{message.EventType}, DocFlow.BuildingBlocks");
                if (eventType is null)
                {
                    message.Error = $"Type not found: {message.EventType}";
                    message.ProcessedAtUtc = DateTime.UtcNow;
                    continue;
                }

                var @event = JsonSerializer.Deserialize(message.Payload, eventType);
                if (@event is null)
                {
                    message.Error = "Failed to deserialize event";
                    message.ProcessedAtUtc = DateTime.UtcNow;
                    continue;
                }

                var publishMethod = typeof(IEventBus).GetMethod("PublishAsync")!.MakeGenericMethod(eventType);
                await (Task)publishMethod.Invoke(eventBus, [@event, message.TopicName, cancellationToken])!;

                message.ProcessedAtUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                logger.LogWarning(ex, "Failed to publish outbox message {MessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
