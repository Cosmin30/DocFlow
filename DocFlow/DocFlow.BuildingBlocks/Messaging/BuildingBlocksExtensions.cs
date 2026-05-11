using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DocFlow.BuildingBlocks.Messaging;

public static class BuildingBlocksServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services, string bootstrapServers)
    {
        services.AddSingleton<IEventBus>(new KafkaEventBus(bootstrapServers));
        return services;
    }

    public static IApplicationBuilder UseDocFlowSerilogRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }

    public static IHostBuilder UseElasticsearchLogging(this IHostBuilder hostBuilder, string elasticUri, string indexFormat)
    {
        hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    IndexFormat = $"{indexFormat}-logs-{{0:yyyy.MM.dd}}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1
                })
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .ReadFrom.Configuration(context.Configuration);
        });

        return hostBuilder;
    }
}
