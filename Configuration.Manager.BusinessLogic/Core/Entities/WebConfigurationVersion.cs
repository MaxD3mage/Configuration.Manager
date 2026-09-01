namespace Configuration.Manager.BusinessLogic.Core.Entities;

/// <summary>
/// Версия конфигурации
/// </summary>
public class WebConfigurationVersion
{
    /// <summary>
    /// Идентификатор версии
    /// </summary>
    public Guid Id { get; set; }    
    
    /// <summary>
    /// Идентификатор конфигурации
    /// </summary> 
    public Guid ConfigurationId { get; set; }
    
    /// <summary>
    /// Номер версии
    /// </summary> 
    public int VersionNumber { get; set; }
    
    /// <summary>
    /// Настройки версии конфигурации json
    /// </summary> 
    public string SettingsJson { get; set; } = "{}";
    
    /// <summary>
    /// Дата создания версии конфигурации
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Комментарий версии конфигурации
    /// </summary>
    public string? Comment { get; set; }
}