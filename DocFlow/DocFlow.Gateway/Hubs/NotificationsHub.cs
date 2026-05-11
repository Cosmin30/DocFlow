using Microsoft.AspNetCore.SignalR;

namespace DocFlow.Gateway.Hubs;

public sealed class NotificationsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.GetHttpContext()?.Request.Query["tenantId"].ToString();
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");
        }

        if (!string.IsNullOrWhiteSpace(tenantId) && !string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:user:{userId}");
        }

        await base.OnConnectedAsync();
    }
}
