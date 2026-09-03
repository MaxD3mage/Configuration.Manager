using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Configuration.Manager.BusinessLogic.App.DTOs;
using Configuration.Manager.BusinessLogic.App.Exceptions;
using Configuration.Manager.BusinessLogic.Core.Entities;
using Configuration.Manager.BusinessLogic.Core.Interfaces;

namespace Configuration.Manager.BusinessLogic.App.Services;


public class ConfigurationService(IConfigurationRepository repository, INotificationService notificationService) : IConfigurationService
{
    public async Task<WebConfigurationDto> CreateAsync(CreateConfigurationDto dto, string userId)
    {
        ValidateCreateDto(dto);

        if (!await repository.IsNameUniqueAsync(userId, dto.Name))
            throw new ConflictException("Configuration name exists");

        var config = new WebConfiguration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Versions = []
        };

        var version = new WebConfigurationVersion
        {
            Id = Guid.NewGuid(),
            ConfigurationId = config.Id,
            VersionNumber = 1,
            SettingsJson = dto.SettingsJson.GetRawText(),
            CreatedAt = DateTime.UtcNow,
            Comment = dto.Comment
        };

        config.Versions.Add(version);
        config.CurrentVersionId = version.Id;

        repository.Add(config);
        await repository.SaveChangesAsync();

        await notificationService.NotifyConfigurationCreatedAsync(userId, config.Id);

        return MapToDto(config, version);
    }

    public async Task<WebConfigurationDto> UpdateAsync(Guid id, UpdateConfigurationDto dto, string userId)
    {
        ValidateUpdateDto(dto);

        var config = await repository.GetByIdAsync(id);
        if (config == null || config.UserId != userId)
            throw new NotFoundException("Configuration not found");

        if (!string.Equals(config.Name, dto.Name, StringComparison.Ordinal))
        {
            if (!await repository.IsNameUniqueAsync(userId, dto.Name, id))
                throw new ConflictException("Configuration name exists");
        }

        var maxVersion = config.Versions.Max(v => v.VersionNumber);
        var newVersion = new WebConfigurationVersion
        {
            Id = Guid.NewGuid(),
            ConfigurationId = config.Id,
            VersionNumber = maxVersion + 1,
            SettingsJson = dto.SettingsJson.GetRawText(),
            CreatedAt = DateTime.UtcNow,
            Comment = dto.Comment
        };

        config.Versions.Add(newVersion);
        config.CurrentVersionId = newVersion.Id;
        config.Name = dto.Name;
        config.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();

        await notificationService.NotifyConfigurationUpdatedAsync(userId, config.Id, newVersion.VersionNumber);

        return MapToDto(config, newVersion);
    }

    public async Task RollbackAsync(Guid configId, Guid versionId, string userId)
    {
        var config = await repository.GetByIdAsync(configId);
        if (config == null || config.UserId != userId)
            throw new NotFoundException("Configuration not found");

        var version = config.Versions.FirstOrDefault(v => v.Id == versionId);
        if (version == null)
            throw new NotFoundException("Version not found in configuration");

        config.CurrentVersionId = versionId;
        config.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();

        await notificationService.NotifyConfigurationUpdatedAsync(userId, configId, version.VersionNumber);
    }

    public async Task<IEnumerable<WebConfigurationDto>> GetListAsync( string userId, string? name, DateTime? from, DateTime? to)
    {
        var configs = await repository.GetByUserIdAsync(userId, name, from, to);
        return configs.Select(c => MapToDto(c, c.Versions.First(v => v.Id == c.CurrentVersionId)));
    }

    public async Task<WebConfigurationDto> GetByIdAsync(Guid id, string userId)
    {
        var config = await repository.GetByIdAsync(id);
        if (config == null || config.UserId != userId)
            throw new NotFoundException("Configuration not found");

        var version = config.Versions.First(v => v.Id == config.CurrentVersionId);
        return MapToDto(config, version);
    }

    private static WebConfigurationDto MapToDto(WebConfiguration config, WebConfigurationVersion version)
    {
        var settingsJsonElement = JsonDocument.Parse(version.SettingsJson).RootElement;
        
        return new WebConfigurationDto
        {
            Id = config.Id,
            Name = config.Name,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt,
            CurrentVersionNumber = version.VersionNumber,
            SettingsJson = settingsJsonElement
        };
    }
    

    private static void ValidateCreateDto(CreateConfigurationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required");
        if (dto.Name.Length > 100)
            throw new ValidationException("Name cannot exceed 100 characters");
        if (dto.SettingsJson.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            throw new ValidationException("SettingsJson is required");
        if (dto.SettingsJson.ValueKind != JsonValueKind.Object)
            throw new ValidationException("SettingsJson must be a JSON object");
    }
    
    private static void ValidateUpdateDto(UpdateConfigurationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Name is required");
        if (dto.Name.Length > 100)
            throw new ValidationException("Name cannot exceed 100 characters");
        if (dto.SettingsJson.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            throw new ValidationException("SettingsJson is required");
        if (dto.SettingsJson.ValueKind != JsonValueKind.Object)
            throw new ValidationException("SettingsJson must be a JSON object");
    }
}

