using Configuration.Manager.BusinessLogic.App.Services;
using Microsoft.AspNetCore.SignalR;
namespace Configuration.Manager.Web.Hubs;

public class ConfigurationHub(IConfigurationService configurationService) : Hub
{
    public async Task Subscribe(string userId, string[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{type}_{userId}");
        }
        try
        {
            var configurations = await configurationService.GetListAsync(userId, null, null, null);
            await Clients.Caller.SendAsync("ConfigurationsList", configurations);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", new { message = $"Failed to load configurations: {ex.Message}" });
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