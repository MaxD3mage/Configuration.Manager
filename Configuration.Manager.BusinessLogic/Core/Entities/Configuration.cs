namespace Configuration.Manager.BusinessLogic.Core.Entities;

/// <summary>
/// Конфигурация
/// </summary>
public class Configuration
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Название конфигурации
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата создания конфигурации
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего изменения конфигурации
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Идентификатор текущей версии
    /// </summary>
    public Guid CurrentVersionId { get; set; }
    
    /// <summary>
    /// Список всех версий конфигурации
    /// </summary>
    public List<ConfigurationVersion> Versions { get; set; } = new();
}