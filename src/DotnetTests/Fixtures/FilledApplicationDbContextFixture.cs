using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Data.SeedData;
using DotnetTests.PersistenceService.Stores;

namespace DotnetTests.Fixtures;

public class FilledApplicationDbContextFixture : IAsyncLifetime
{
    public ApplicationDbContext context { get; private set; }

    private readonly TestSeeder _testSeeder;

    public FilledApplicationDbContextFixture()
    {
        var envFilePath = Directory.GetCurrentDirectory() + "/../../../.env";
        if (System.IO.File.Exists(envFilePath))
        {
            using (StreamReader reader = new StreamReader(envFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    int i = line.IndexOf("=");
                    if (i == -1)
                    {
                        throw new InvalidOperationException(
                            "Invalid .env file format"
                        );
                    }
                    Environment.SetEnvironmentVariable(
                        line.Substring(0, i),
                        line.Substring(i + 1)
                    );
                }
            }
        }
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
        _testSeeder = new TestSeeder(context, UserStoreTests.GetUserStore());
    }

    public async Task InitializeAsync()
    {
        await _testSeeder.Seed(TestSeeder.Small);
    }

    public async Task DisposeAsync()
    {
        context.Database.EnsureDeleted();
        await context.DisposeAsync();
    }
}

[CollectionDefinition("Database collection 2")]
public class DatabaseCollection2
    : ICollectionFixture<FilledApplicationDbContextFixture> { }
