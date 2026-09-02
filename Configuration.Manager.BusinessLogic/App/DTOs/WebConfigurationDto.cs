using System.Text.Json;

namespace Configuration.Manager.BusinessLogic.App.DTOs;

public class WebConfigurationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int CurrentVersionNumber { get; set; }
    public JsonElement SettingsJson { get; set; }
}