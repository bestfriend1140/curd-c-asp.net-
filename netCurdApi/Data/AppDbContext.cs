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
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>()
            .HasKey(t => t.Id);

        // User: Primary Key + Unique Username / Email
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.HasIndex(u => u.Username).IsUnique();
            user.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4",
                Status = "A",
                CreatedAt = new DateTime(2026, 11, 11, 8, 0, 0)
            },
            new User
            {
                Id = 2,
                Username = "alice",
                Email = "alice@example.com",
                PasswordHash = "3d5e9ac865d0e0ec9a56fa0f8739772b001269721c5776eb4dfe25c1eab1919f",
                Status = "A",
                CreatedAt = new DateTime(2026, 11, 11, 8, 15, 0)
            },
            new User
            {
                Id = 3,
                Username = "bob",
                Email = "bob@example.com",
                PasswordHash = "fc6e5ceb0061b7059f2d38619c03c7b2f4f1ca6e0d29be5b2c0ea7f7d903c197",
                Status = "I",
                CreatedAt = new DateTime(2026, 11, 11, 8, 30, 0)
            }
        );

        modelBuilder.Entity<TodoItem>().HasData(
            new TodoItem
            {
                Id = "todo_01",
                UserId = 1,
                Title = "Learn ASP.NET Core",
                Description = "Learn Web API and Entity Framework Core",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 9, 0, 0),
                Status = "A"
            },
            new TodoItem
            {
                Id = "todo_02",
                UserId = 1,
                Title = "Learn Angular",
                Description = "Learn Angular components and services",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 9, 30, 0),
                Status = "A"
            },
            new TodoItem
            {
                Id = "todo_03",
                UserId = 2,
                Title = "Create CRUD API",
                Description = "Implement Create, Read, Update and Delete",
                IsCompleted = true,
                CreatedAt = new DateTime(2026, 11, 11, 10, 0, 0),
                Status = "A"
            },
            new TodoItem
            {
                Id = "todo_04",
                UserId = 2,
                Title = "Push Project to GitHub",
                Description = "Upload source code and migrations",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 11, 11, 10, 30, 0),
                Status = "A"
            },
            new TodoItem
            {
                Id = "todo_05",
                UserId = 3,
                Title = "Test API with Postman",
                Description = "Test all CRUD endpoints",
                IsCompleted = true,
                CreatedAt = new DateTime(2026, 11, 11, 11, 0, 0),
                Status = "A"
            }
        );
    }
}