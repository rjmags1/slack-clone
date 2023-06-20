using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using DotNetEnv;

namespace DotnetTests.Fixtures;

public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext context { get; private set; }

    public ApplicationDbContextFixture()
    {
        string connectionString =
            "Server=localhost;Port=5432;Database=slack_clone_test;Username=postgres;Password=postgres;IncludeErrorDetail=true";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            //.EnableSensitiveDataLogging()
            //.LogTo(Console.WriteLine)
            .Options;

        ApplicationDbContext c = new ApplicationDbContext(options);
        c.Database.Migrate();
        context = c;
    }

    public void Dispose()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection
    : ICollectionFixture<ApplicationDbContextFixture> { }
