using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Data.SeedData;
using DotnetTests.PersistenceService.Stores;
using DotnetTest.SetupUtils;

namespace DotnetTests.Fixtures;

public class FilledApplicationDbContextFixture : IAsyncLifetime
{
    public ApplicationDbContext Context { get; private set; }

    private readonly TestSeeder _testSeeder;

    private readonly bool _preserveSeededData;

    private readonly bool _envIsDev;

    private readonly bool _doSeed;

    public FilledApplicationDbContextFixture()
    {
        SetupUtils.LoadEnvironmentVariables("/../../../.env");

        _doSeed = Environment.GetEnvironmentVariable("SEED") == "true";
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

        ApplicationDbContext c = new(options);
        if (!c.Database.CanConnect())
        {
            c.Database.Migrate();
        }
        Context = c;
        _testSeeder = new TestSeeder(Context, UserStoreTests1.GetUserStore());
    }

    public async Task InitializeAsync()
    {
        if (_doSeed)
        {
            string seedSize = Environment.GetEnvironmentVariable(
                "LARGE_SEED_SIZE"
            )
                is null
                ? TestSeeder.Small
                : TestSeeder.Large;
            await _testSeeder.Seed(seedSize);
        }
    }

    public async Task DisposeAsync()
    {
        if (!_preserveSeededData)
        {
            Context.Database.EnsureDeleted();
        }
        await Context.DisposeAsync();
    }
}

[CollectionDefinition("Filled Database Test Collection")]
public class DatabaseCollection2
    : ICollectionFixture<FilledApplicationDbContextFixture> { }
