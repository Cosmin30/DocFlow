using DocFlow.AuditService.Application.Services;
using DocFlow.AuditService.Infrastructure.Persistence;
using DocFlow.AuditService.Infrastructure.Messaging;
using DocFlow.AuditService.Infrastructure.Repositories;
using DocFlow.BuildingBlocks.Security;
using DocFlow.BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseElasticsearchLogging(
    elasticUri: builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200",
    indexFormat: "docflow-audit-service"
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDocFlowJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddHostedService<KafkaAuditConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
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
