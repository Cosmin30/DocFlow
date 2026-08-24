using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;

namespace DocFlow.BuildingBlocks.Resilience;

public static class ResilienceExtensions
{
    public static IServiceCollection AddDocFlowResilience(this IServiceCollection services)
    {
        services.AddResiliencePipeline("default", builder =>
        {
            builder
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>()
                        .Handle<TimeoutRejectedException>()
                        .Handle<Exception>(ex => ex is TimeoutException)
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
        });

        return services;
    }
}
