using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Stores;

namespace DotnetTests.Fixtures;

public class FilledApplicationDbContextFixture : IAsyncLifetime
{
    public ApplicationDbContext context { get; private set; }

    private readonly ThemeStore _themeStore;

    public FilledApplicationDbContextFixture()
    {
        var envFilePath = Directory.GetCurrentDirectory() + "/../../../.env";
        if (File.Exists(envFilePath))
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
        _themeStore = new ThemeStore(context);
    }

    public async Task InitializeAsync()
    {
        await _themeStore.InsertShippedThemes();
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
