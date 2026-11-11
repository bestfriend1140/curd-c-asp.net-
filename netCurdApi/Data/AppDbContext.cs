using Microsoft.EntityFrameworkCore;
using netCurdApi.Models;

namespace netCurdApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>().HasData(
            new TodoItem
            {
                Id = 1,
                Title = "Learn ASP.NET Core",
                Description = "Learn Web API and Entity Framework Core",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 9, 0, 0)
            },
            new TodoItem
            {
                Id = 2,
                Title = "Learn Angular",
                Description = "Learn Angular components and services",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 9, 30, 0)
            },
            new TodoItem
            {
                Id = 3,
                Title = "Create CRUD API",
                Description = "Implement Create, Read, Update and Delete",
                IsCompleted = true,
                CreatedAt = new DateTime(2026, 11, 11, 10, 0, 0)
            },
            new TodoItem
            {
                Id = 4,
                Title = "Push Project to GitHub",
                Description = "Upload source code and migrations",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 10, 30, 0)
            },
            new TodoItem
            {
                Id = 5,
                Title = "Test API with Postman",
                Description = "Test all CRUD endpoints",
                IsCompleted = true,
                CreatedAt = new DateTime(2026, 11, 11, 11, 0, 0)
            }
        );
    }
}