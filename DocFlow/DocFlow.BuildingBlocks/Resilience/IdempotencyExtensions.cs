using Microsoft.AspNetCore.Builder;

namespace DocFlow.BuildingBlocks.Resilience;

public static class IdempotencyExtensions
{
    public static WebApplication UseIdempotency(this WebApplication app)
    {
        app.UseMiddleware<IdempotencyMiddleware>();
        return app;
    }
}
