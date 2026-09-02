using Microsoft.AspNetCore.SignalR;
namespace Configuration.Manager.Web.Hubs;

public class ConfigurationHub : Hub
{
    public async Task Subscribe(string userId, string[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{type}_{userId}");
        }
    }

    public async Task Unsubscribe(string userId, string[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{type}_{userId}");
        }
    }
}