using DocFlow.BuildingBlocks.Security;
using DocFlow.BuildingBlocks.Messaging;
using DocFlow.BuildingBlocks.Messaging.Outbox;
using DocFlow.BuildingBlocks.Resilience;
using DocFlow.DocumentService.Application.Abstractions;
using DocFlow.DocumentService.Application.Behaviors;
using DocFlow.DocumentService.Infrastructure.Persistence;
using DocFlow.DocumentService.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseElasticsearchLogging(
    elasticUri: builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200",
    indexFormat: "docflow-document-service"
);

builder.Services.AddKafkaEventBus(builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOutbox<DocumentDbContext>();
builder.Services.AddDocFlowJwtAuthentication(builder.Configuration);
builder.Services.AddDocFlowHealthChecks("DocFlow.DocumentService");
builder.Services.AddDocFlowResilience();
builder.Services.AddMemoryCache();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();
    db.Database.Migrate();
    await db.SeedAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDocFlowSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDocFlowHealthChecks();

app.Run();
