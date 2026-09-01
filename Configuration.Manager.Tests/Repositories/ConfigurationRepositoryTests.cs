using Configuration.Manager.BusinessLogic.Core.Entities;
using Configuration.Manager.BusinessLogic.Repository.Data;
using Configuration.Manager.BusinessLogic.Repository.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Manager.Tests.Repositories;

[TestFixture]
public class ConfigurationRepositoryTests
{
    private AppDbContext _context;
    private ConfigurationRepository _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new ConfigurationRepository(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private WebConfiguration CreateTestConfig(string userId = "user1", string name = "Test")
    {
        var config = new WebConfiguration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var version = new WebConfigurationVersion
        {
            Id = Guid.NewGuid(),
            ConfigurationId = config.Id,
            VersionNumber = 1,
            SettingsJson = "{}",
            CreatedAt = DateTime.UtcNow
        };
        config.Versions = [version];
        config.CurrentVersionId = version.Id;
        return config;
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnConfigWithVersions()
    {
        var config = CreateTestConfig();
        _context.Configurations.Add(config);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(config.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(config.Id);
        result.Name.Should().Be(config.Name);
        result.UserId.Should().Be(config.UserId);
        result.CurrentVersionId.Should().Be(config.CurrentVersionId);
        result.Versions.Should().HaveCount(1);
    }

    [Test]
    public async Task IsNameUniqueAsync_ShouldReturnTrueIfUnique()
    {
        var config = CreateTestConfig("user1", "Unique");
        _context.Configurations.Add(config);
        await _context.SaveChangesAsync();

        var unique = await _repository.IsNameUniqueAsync("user1", "NewName");
        unique.Should().BeTrue();
    }

    [Test]
    public async Task IsNameUniqueAsync_ShouldReturnFalseIfDuplicate()
    {
        var config = CreateTestConfig("user1", "Duplicate");
        _context.Configurations.Add(config);
        await _context.SaveChangesAsync();

        var unique = await _repository.IsNameUniqueAsync("user1", "Duplicate");
        unique.Should().BeFalse();
    }

    [Test]
    public async Task GetByUserIdAsync_FiltersByNameAndDate()
    {
        var c1 = CreateTestConfig("user1", "Alpha");
        c1.CreatedAt = new DateTime(2025, 1, 1);
        var c2 = CreateTestConfig("user1", "Beta");
        c2.CreatedAt = new DateTime(2025, 2, 1);
        _context.Configurations.AddRange(c1, c2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserIdAsync("user1", "Alpha", new DateTime(2024, 12, 1), new DateTime(2025, 1, 15));

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Alpha");
    }
}