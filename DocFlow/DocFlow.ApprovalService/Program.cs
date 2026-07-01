using DocFlow.ApprovalService.Application.Services;
using DocFlow.ApprovalService.Infrastructure.Persistence;
using DocFlow.ApprovalService.Infrastructure.Repositories;
using DocFlow.BuildingBlocks.Security;
using DocFlow.BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseElasticsearchLogging(
    elasticUri: builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200",
    indexFormat: "docflow-approval-service"
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApprovalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDocFlowJwtAuthentication(builder.Configuration);

builder.Services.AddKafkaEventBus(builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092");

builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApprovalDbContext>();
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

app.Run();
