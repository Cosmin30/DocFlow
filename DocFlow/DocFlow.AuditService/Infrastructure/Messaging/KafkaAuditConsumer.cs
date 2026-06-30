using System.Text.Json;
using Confluent.Kafka;
using DocFlow.AuditService.Application.Contracts;
using DocFlow.AuditService.Application.Services;
using DocFlow.BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocFlow.AuditService.Infrastructure.Messaging;

public sealed class KafkaAuditConsumer(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<KafkaAuditConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "docflow-audit-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        var topics = new[]
        {
            "docflow.document.created",
            "docflow.approval.requested",
            "docflow.approval.decided"
        };

        consumer.Subscribe(topics);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result?.Message?.Value is null)
                {
                    continue;
                }

                await HandleMessageAsync(result.Topic, result.Message.Value, cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka audit consumer error");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        consumer.Close();
    }

    private Task HandleMessageAsync(string topic, string json, CancellationToken cancellationToken)
    {
        return topic switch
        {
            "docflow.document.created" => HandleDocumentCreatedAsync(json, cancellationToken),
            "docflow.approval.requested" => HandleApprovalRequestedAsync(json, cancellationToken),
            "docflow.approval.decided" => HandleApprovalDecidedAsync(json, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private async Task HandleDocumentCreatedAsync(string json, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<DocumentCreatedIntegrationEvent>(json);
        if (evt is null)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        await auditService.WriteAsync(
            evt.TenantId,
            evt.OwnerUserId,
            ipAddress: null,
            device: null,
            new WriteAuditRequest(
                Action: "DocumentCreated",
                EntityType: "Document",
                EntityId: evt.DocumentId.ToString(),
                MetadataJson: json),
            cancellationToken);
    }

    private async Task HandleApprovalRequestedAsync(string json, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<ApprovalRequestedIntegrationEvent>(json);
        if (evt is null)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        await auditService.WriteAsync(
            evt.TenantId,
            evt.RequestedByUserId,
            ipAddress: null,
            device: null,
            new WriteAuditRequest(
                Action: "ApprovalRequested",
                EntityType: "ApprovalRequest",
                EntityId: evt.ApprovalId.ToString(),
                MetadataJson: json),
            cancellationToken);
    }

    private async Task HandleApprovalDecidedAsync(string json, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<ApprovalDecidedIntegrationEvent>(json);
        if (evt is null)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        await auditService.WriteAsync(
            evt.TenantId,
            evt.DecidedByUserId,
            ipAddress: null,
            device: null,
            new WriteAuditRequest(
                Action: evt.Approved ? "ApprovalApproved" : "ApprovalRejected",
                EntityType: "ApprovalRequest",
                EntityId: evt.ApprovalId.ToString(),
                MetadataJson: json),
            cancellationToken);
    }
}
