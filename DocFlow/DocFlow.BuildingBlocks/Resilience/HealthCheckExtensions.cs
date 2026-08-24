using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace DocFlow.BuildingBlocks.Resilience;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddDocFlowHealthChecks(this IServiceCollection services, string serviceName)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        return services;
    }

    public static WebApplication MapDocFlowHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}

internal static class UIResponseWriter
{
    public static async Task WriteHealthCheckUIResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";
        var response = new
        {
            status = result.Status.ToString(),
            checks = result.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description
            })
        };
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
