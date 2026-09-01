namespace Configuration.Manager.BusinessLogic.App.Services;

public interface INotificationService
{
    Task NotifyConfigurationCreatedAsync(string userId, Guid configId);
    Task NotifyConfigurationUpdatedAsync(string userId, Guid configId, int newVersion);
}