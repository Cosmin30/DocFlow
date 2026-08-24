using DocFlow.BuildingBlocks.Messaging;
using DocFlow.BuildingBlocks.Security;
using DocFlow.BuildingBlocks.Resilience;
using Serilog;
using DocFlow.Gateway.Hubs;
using DocFlow.Gateway.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseElasticsearchLogging(
    elasticUri: builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200",
    indexFormat: "docflow-gateway"
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDocFlowJwtAuthentication(builder.Configuration);
builder.Services.AddDocFlowHealthChecks("DocFlow.Gateway");
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddSignalR();

builder.Services.AddHostedService<KafkaNotificationsConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDocFlowSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { name = "DocFlow Gateway", status = "running" }));
app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapDocFlowHealthChecks();

app.MapReverseProxy().RequireAuthorization();

app.Run();
