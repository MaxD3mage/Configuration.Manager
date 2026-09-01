using Configuration.Manager.BusinessLogic.App.DTOs;

namespace Configuration.Manager.BusinessLogic.App.Services;

public interface IConfigurationService
{
    Task<WebConfigurationDto> CreateAsync(CreateConfigurationDto dto, string userId);
    Task<WebConfigurationDto> UpdateAsync(Guid id, UpdateConfigurationDto dto, string userId);
    Task RollbackAsync(Guid configId, Guid versionId, string userId);
    Task<IEnumerable<WebConfigurationDto>> GetListAsync(string userId, string? name, DateTime? from, DateTime? to);
    Task<WebConfigurationDto> GetByIdAsync(Guid id, string userId);
}