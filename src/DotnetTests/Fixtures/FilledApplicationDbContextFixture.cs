using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Data.SeedData;
using DotnetTests.PersistenceService.Stores;
using DotnetTest.SetupUtils;

namespace DotnetTests.Fixtures;

public class FilledApplicationDbContextFixture : IAsyncLifetime
{
    public ApplicationDbContext context { get; private set; }

    private readonly TestSeeder _testSeeder;

    private readonly bool _preserveSeededData;

    private readonly bool _envIsDev;

    public FilledApplicationDbContextFixture()
    {
        SetupUtils.LoadEnvironmentVariables("/../../../.env");

        _preserveSeededData =
            Environment.GetEnvironmentVariable("PRESERVE_SEEDED_DATA")
            == "true";
        _envIsDev = Environment.GetEnvironmentVariable("ENV") == "dev";
        string connectionString = _envIsDev
            ? "LOCAL_DB_CONNECTION_STRING"
            : "TEST_DB_CONNECTION_STRING";
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable(connectionString))
            //.EnableSensitiveDataLogging()
            //.LogTo(Console.WriteLine)
            .Options;

        ApplicationDbContext c = new ApplicationDbContext(options);
        if (!c.Database.CanConnect())
        {
            c.Database.Migrate();
        }
        context = c;
        _testSeeder = new TestSeeder(context, UserStoreTests.GetUserStore());
    }

    public async Task InitializeAsync()
    {
        string seedSize = Environment.GetEnvironmentVariable("LARGE_SEED_SIZE")
            is null
            ? TestSeeder.Small
            : TestSeeder.Large;
        await _testSeeder.Seed(seedSize);
    }

    public async Task DisposeAsync()
    {
        if (!_preserveSeededData)
        {
            context.Database.EnsureDeleted();
        }
        await context.DisposeAsync();
    }
}

[CollectionDefinition("Database collection 2")]
public class DatabaseCollection2
    : ICollectionFixture<FilledApplicationDbContextFixture> { }
