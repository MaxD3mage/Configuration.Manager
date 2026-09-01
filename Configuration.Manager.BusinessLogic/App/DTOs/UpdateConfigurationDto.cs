namespace Configuration.Manager.BusinessLogic.App.DTOs;

public class UpdateConfigurationDto
{
    public string Name { get; set; } = string.Empty;
    public string SettingsJson { get; set; } = "{}";
    public string? Comment { get; set; }
}