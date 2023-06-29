using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;

namespace DotnetTests.Fixtures;

public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext context { get; private set; }

    public ApplicationDbContextFixture()
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
