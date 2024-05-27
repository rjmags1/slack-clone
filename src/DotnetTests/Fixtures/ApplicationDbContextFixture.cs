using DotnetTest.SetupUtils;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;

namespace DotnetTests.Fixtures;

public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public ApplicationDbContextFixture()
    {
        SetupUtils.LoadEnvironmentVariables("/../../../.env");

        bool dev = Environment.GetEnvironmentVariable("ENV") == "dev";
        string connectionString = dev
            ? "LOCAL_DB_CONNECTION_STRING"
            : "TEST_DB_CONNECTION_STRING";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable(connectionString))
            //.EnableSensitiveDataLogging()
            //.LogTo(Console.WriteLine)
            .Options;

        ApplicationDbContext c = new(options);
        c.Database.Migrate();
        Context = c;
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

[CollectionDefinition("Empty Database Test Collection")]
public class DatabaseCollection1
    : ICollectionFixture<ApplicationDbContextFixture> { }
