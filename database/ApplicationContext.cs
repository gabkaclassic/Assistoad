using System.Data.Entity.Core.Common.CommandTrees;
using AssisToad.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Task = AssisToad.Entities.Task;

namespace AssisToad.database;

public sealed class ApplicationContext : DbContext
{

    private static readonly string? server;
    private static readonly string? database;
    private static readonly string? user;
    private static readonly string? password;
    public DbSet<Task> Tasks { set; get; }
    public DbSet<Category> Categories { set; get; }

    public DbSet<User> Users { set; get; }
    
    public DbSet<Settings> Settings { set; get; }

    static ApplicationContext()
    {
        server = Environment.GetEnvironmentVariable("server");
        database = Environment.GetEnvironmentVariable("database");
        user = Environment.GetEnvironmentVariable("user");
        password = Environment.GetEnvironmentVariable("password");
    }

    public ApplicationContext() : base()
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<Task>().HasKey(e => e.Id);
        modelBuilder.Entity<User>().HasKey(e => e.ChatId);
        modelBuilder.Entity<Settings>().HasKey(e => e.Id);
        modelBuilder.Entity<Category>().HasKey(e => e.Id);
        modelBuilder.Entity<Task>().HasOne(e => e.Category);
        modelBuilder.Entity<User>().HasMany(e => e.Categories).WithOne(e => e.Owner);
        modelBuilder.Entity<User>().HasMany(e => e.Tasks).WithOne(e => e.Owner);
        modelBuilder.Entity<User>().HasOne(e => e.Settings);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql($"Server={server};Database={database};User ID={user};Password={password};");
    }
}