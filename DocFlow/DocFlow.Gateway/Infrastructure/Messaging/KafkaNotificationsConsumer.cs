using System.Text.Json;
using Confluent.Kafka;
using DocFlow.BuildingBlocks.Messaging.Events;
using DocFlow.Gateway.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocFlow.Gateway.Infrastructure.Messaging;

public sealed class KafkaNotificationsConsumer(
    IHubContext<NotificationsHub> hubContext,
    IConfiguration configuration,
    ILogger<KafkaNotificationsConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "docflow-gateway-notifications",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("docflow.notifications");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result?.Message?.Value is null)
                {
                    continue;
                }

                var evt = JsonSerializer.Deserialize<NotificationIntegrationEvent>(result.Message.Value);
                if (evt is null)
                {
                    continue;
                }

                var payload = new
                {
                    evt.TenantId,
                    evt.UserId,
                    evt.Title,
                    evt.Message,
                    evt.CreatedAtUtc
                };

                var tenantGroup = $"tenant:{evt.TenantId}";

                if (evt.UserId is null)
                {
                    await hubContext.Clients.Group(tenantGroup)
                        .SendAsync("notification", payload, cancellationToken: stoppingToken);
                }
                else
                {
                    await hubContext.Clients.Group($"tenant:{evt.TenantId}:user:{evt.UserId}")
                        .SendAsync("notification", payload, cancellationToken: stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka notifications consumer error");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        consumer.Close();
    }
}
