using Configuration.Manager.BusinessLogic.App.DTOs;
using Configuration.Manager.BusinessLogic.App.Services;
using Configuration.Manager.BusinessLogic.Core.Entities;
using Configuration.Manager.BusinessLogic.Core.Interfaces;
using Configuration.Manager.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace Configuration.Manager.Tests.Services;

[TestFixture]
public class ConfigurationServiceTests
{
    private Mock<IConfigurationRepository> _repoMock;
    private Mock<INotificationService> _notificationMock;
    private ConfigurationService _service;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IConfigurationRepository>();
        _notificationMock = new Mock<INotificationService>();
        _service = new ConfigurationService(_repoMock.Object, _notificationMock.Object);
    }

    private WebConfiguration CreateTestConfig(string userId = "user1", string name = "Test", int version = 1, string json = "{}")
    {
        var config = new WebConfiguration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Versions = []
        };
        var v = new WebConfigurationVersion
        {
            Id = Guid.NewGuid(),
            ConfigurationId = config.Id,
            VersionNumber = version,
            SettingsJson = json,
            CreatedAt = DateTime.UtcNow
        };
        config.Versions.Add(v);
        config.CurrentVersionId = v.Id;
        return config;
    }

    [Test]
    public async Task CreateAsync_ValidData_ShouldCreateAndReturnDto()
    {
        var dto = new CreateConfigurationDto { Name = "Test", SettingsJson = JsonHelper.CreateJsonElement("{}") };
        const string userId = "user1";

        _repoMock.Setup(r => r.IsNameUniqueAsync(userId, dto.Name, null))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.Add(It.IsAny<WebConfiguration>()));
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto, userId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        _notificationMock.Verify(n => n.NotifyConfigurationCreatedAsync(userId, It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ValidData_ShouldUpdateAndReturnDto()
    {
        var config = CreateTestConfig("user1", "Old");
        var dto = new UpdateConfigurationDto { Name = "New", SettingsJson = JsonHelper.CreateJsonElement("{\"new\":\"data\"}") };
        const string userId = "user1";

        _repoMock.Setup(r => r.GetByIdAsync(config.Id))
            .ReturnsAsync(config);
        _repoMock.Setup(r => r.IsNameUniqueAsync(userId, dto.Name, config.Id))
            .ReturnsAsync(true);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(config.Id, dto, userId);

        result.Name.Should().Be("New");
        result.CurrentVersionNumber.Should().Be(2);
        _notificationMock.Verify(n => n.NotifyConfigurationUpdatedAsync(userId, config.Id, 2), Times.Once);
    }

    [Test]
    public async Task RollbackAsync_ValidVersion_ShouldSwitchVersion()
    {
        var config = CreateTestConfig();
        var v2 = new WebConfigurationVersion
        {
            Id = Guid.NewGuid(),
            ConfigurationId = config.Id,
            VersionNumber = 2,
            SettingsJson = "{\"new\":\"data\"}",
            CreatedAt = DateTime.UtcNow
        };
        config.Versions.Add(v2);
        config.CurrentVersionId = v2.Id;

        _repoMock.Setup(r => r.GetByIdAsync(config.Id))
            .ReturnsAsync(config);
        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var targetVersionId = config.Versions.First().Id;
        await _service.RollbackAsync(config.Id, targetVersionId, "user1");

        config.CurrentVersionId.Should().Be(targetVersionId);
        _notificationMock.Verify(n => n.NotifyConfigurationUpdatedAsync("user1", config.Id, 1), Times.Once);
    }

    [Test]
    public async Task GetListAsync_ShouldReturnFilteredConfigurations()
    {
        var configs = new List<WebConfiguration>
        {
            CreateTestConfig("user1", "Alpha"),
            CreateTestConfig("user1", "Beta")
        };
        _repoMock.Setup(r => r.GetByUserIdAsync("user1", "Alpha", null, null))
            .ReturnsAsync(configs.Where(c => c.Name == "Alpha"));

        var result = await _service.GetListAsync("user1", "Alpha", null, null);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Alpha");
    }

    [Test]
    public async Task GetByIdAsync_Existing_ShouldReturnDto()
    {
        const string json = "{\"data\":\"value\"}";
        var config = CreateTestConfig("user1", "Test", 1, json);
        _repoMock.Setup(r => r.GetByIdAsync(config.Id))
            .ReturnsAsync(config);

        var result = await _service.GetByIdAsync(config.Id, "user1");

        result.Should().NotBeNull();
        result.Id.Should().Be(config.Id);
        result.SettingsJson.GetRawText().Should().Be(json);
    }
}
