using Microsoft.EntityFrameworkCore;
using TaskEntity = TaskManagement.Api.Models.Task;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.MajorCode, x.MinorCode }).IsUnique();
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Status)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lookup>().HasData(
            new Lookup { Id = 1, MajorCode = 1, MinorCode = 0, Name = "Task Status" },
            new Lookup { Id = 2, MajorCode = 1, MinorCode = 1, Name = "Initiated" },
            new Lookup { Id = 3, MajorCode = 1, MinorCode = 2, Name = "In Progress" },
            new Lookup { Id = 4, MajorCode = 1, MinorCode = 3, Name = "Completed" },
            new Lookup { Id = 5, MajorCode = 1, MinorCode = 4, Name = "Cancelled" }
        );
    }
}
