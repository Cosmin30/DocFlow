using DocFlow.BuildingBlocks.Messaging;
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
app.MapGet("/", () => Results.Ok(new { name = "DocFlow Gateway", status = "running" }));
app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapReverseProxy();

app.Run();
