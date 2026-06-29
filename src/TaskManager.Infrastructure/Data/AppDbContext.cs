using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.Property(t => t.Category).HasMaxLength(100);
            entity.Property(t => t.AiSuggestion).HasMaxLength(2000);
            entity.Property(t => t.Priority).HasConversion<string>();
            entity.Property(t => t.Status).HasConversion<string>();
        });
    }
}
