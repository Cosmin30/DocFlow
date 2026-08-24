using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace DocFlow.BuildingBlocks.Resilience;

public static class ResilienceExtensions
{
    public static IServiceCollection AddDocFlowResilience(this IServiceCollection services)
    {
        services.AddResilienceEnricher();
        return services;
    }

    private static IServiceCollection AddResilienceEnricher(this IServiceCollection services)
    {
        return services;
    }
}

public static class ResilientHttpClient
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public static async Task<HttpResponseMessage> SendWithRetryAsync(
        Func<CancellationToken, Task<HttpResponseMessage>> action,
        CancellationToken cancellationToken = default)
    {
        return await RetryPolicy.ExecuteAsync(() => action(cancellationToken));
    }
}
