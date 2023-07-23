using DotnetTest.SetupUtils;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;

namespace DotnetTests.Fixtures;

public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext context { get; private set; }

    public ApplicationDbContextFixture()
    {
        SetupUtils.LoadEnvironmentVariables("/../../../.env");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                Environment.GetEnvironmentVariable("TEST_DB_CONNECTION_STRING")
            )
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

[CollectionDefinition("Database collection 1")]
public class DatabaseCollection1
    : ICollectionFixture<ApplicationDbContextFixture> { }
