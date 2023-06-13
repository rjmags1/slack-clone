using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PersistenceService.Models;

namespace PersistenceService.Data.ApplicationDb;

#pragma warning disable CS8618
public class ApplicationDbContext : DbContext
{
    DbSet<Theme> Themes { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //
    }
}

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();
        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
