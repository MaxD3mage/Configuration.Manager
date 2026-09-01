using Configuration.Manager.BusinessLogic.Core.Entities;
using Configuration.Manager.BusinessLogic.Core.Interfaces;
using Configuration.Manager.BusinessLogic.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Manager.BusinessLogic.Repository.Repositories;

public class ConfigurationRepository(AppDbContext context) : IConfigurationRepository
{
    public async Task<WebConfiguration?> GetByIdAsync(Guid id)
    {
        return await context.Configurations
            .Include(c => c.Versions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<WebConfiguration>> GetByUserIdAsync(string userId, string? nameFilter = null, DateTime? from = null, DateTime? to = null)
    {
        var query = context.Configurations
            .Include(c => c.Versions)
            .Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(c => c.Name.Contains(nameFilter));

        if (from.HasValue)
            query = query.Where(c => c.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.CreatedAt <= to.Value);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string userId, string name, Guid? excludeId = null)
    {
        var query = context.Configurations.Where(c => c.UserId == userId && c.Name == name);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync();
    }

    public void Add(WebConfiguration configuration)
    {
        context.Configurations.Add(configuration);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}