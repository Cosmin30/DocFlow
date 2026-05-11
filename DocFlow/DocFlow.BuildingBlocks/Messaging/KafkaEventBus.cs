using System.Text.Json;
using Confluent.Kafka;

namespace DocFlow.BuildingBlocks.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T integrationEvent, string topicName, CancellationToken cancellationToken = default);
}

public class KafkaEventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventBus(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T integrationEvent, string topicName, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(integrationEvent)
        };

        await _producer.ProduceAsync(topicName, message, cancellationToken);
    }

    public void Dispose()
    {
        // Block until all outstanding produce requests have completed
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
