namespace Configuration.Manager.BusinessLogic.Core.Interfaces;

public interface IConfigurationRepository
{
    Task<Entities.WebConfiguration?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.WebConfiguration>> GetByUserIdAsync(string userId, string? nameFilter, DateTime? from, DateTime? to);
    Task<bool> IsNameUniqueAsync(string userId, string name, Guid? excludeId = null);
    void Add(Entities.WebConfiguration webConfiguration);
    void AddVersion(Entities.WebConfigurationVersion version);
    Task SaveChangesAsync();
}