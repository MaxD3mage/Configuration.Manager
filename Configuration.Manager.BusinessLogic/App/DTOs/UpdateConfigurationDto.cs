using System.Text.Json;

namespace Configuration.Manager.BusinessLogic.App.DTOs;

public class UpdateConfigurationDto
{
    public string Name { get; set; } = string.Empty;
    public JsonElement SettingsJson { get; set; }
    public string? Comment { get; set; }
}