using Microsoft.AspNetCore.SignalR;
using Configuration.Manager.BusinessLogic.App.Services;
using Configuration.Manager.Web.Hubs;

namespace Configuration.Manager.Web.Services;

public class NotificationService(IHubContext<ConfigurationHub> hubContext) : INotificationService
{
    public async Task NotifyConfigurationCreatedAsync(string userId, Guid configId)
    {
        await hubContext.Clients.Group($"Created_{userId}")
            .SendAsync("ConfigurationCreated", configId);
    }

    public async Task NotifyConfigurationUpdatedAsync(string userId, Guid configId, int newVersion)
    {
        await hubContext.Clients.Group($"Updated_{userId}")
            .SendAsync("ConfigurationUpdated", new { configId, newVersion });
    }
}