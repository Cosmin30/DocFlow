using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace DocFlow.BuildingBlocks.Resilience;

public sealed class IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method))
        {
            if (context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey)
                && !string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var key = idempotencyKey.ToString();
                var lockObj = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

                await lockObj.WaitAsync();
                try
                {
                    if (cache.TryGetValue($"idempotency:{key}", out object? cachedResult))
                    {
                        if (cachedResult is not null)
                        {
                            context.Response.StatusCode = 200;
                            await context.Response.WriteAsJsonAsync(cachedResult);
                            return;
                        }
                    }

                    var originalBodyStream = context.Response.Body;
                    using var memoryStream = new MemoryStream();
                    context.Response.Body = memoryStream;

                    await next(context);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBodyStream);

                    if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    {
                        cache.Set($"idempotency:{key}", responseBody, TimeSpan.FromMinutes(10));
                    }
                }
                finally
                {
                    lockObj.Release();
                    _locks.TryRemove(key, out _);
                }

                return;
            }
        }

        await next(context);
    }
}
