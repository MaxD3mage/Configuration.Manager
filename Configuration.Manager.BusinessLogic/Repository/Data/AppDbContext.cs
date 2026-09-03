using Configuration.Manager.BusinessLogic.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Configuration.Manager.BusinessLogic.Repository.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WebConfiguration> Configurations { get; set; }
    public DbSet<WebConfigurationVersion> ConfigurationVersions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WebConfiguration>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            entity.HasMany(c => c.Versions)
                .WithOne()
                .HasForeignKey(v => v.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<WebConfigurationVersion>()
                .WithMany()
                .HasForeignKey(c => c.CurrentVersionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<WebConfigurationVersion>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasIndex(v => new { v.ConfigurationId, v.VersionNumber }).IsUnique();
        });
    }
}