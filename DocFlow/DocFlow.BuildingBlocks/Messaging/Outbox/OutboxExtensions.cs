using DocFlow.BuildingBlocks.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocFlow.BuildingBlocks.Messaging.Outbox;

public static class OutboxExtensions
{
    public static IServiceCollection AddOutbox<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IOutboxStore, OutboxStore<TDbContext>>();
        services.AddHostedService<OutboxProcessor<TDbContext>>();
        return services;
    }

    public static ModelBuilder AddOutboxModelCreating(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProcessedAtUtc);
        });
        return modelBuilder;
    }
}
