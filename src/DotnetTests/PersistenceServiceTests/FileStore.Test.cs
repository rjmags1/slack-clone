using PersistenceService.Stores;
using PersistenceService.Data.ApplicationDb;
using Moq;
using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;

using Models = PersistenceService.Models;

namespace DotnetTests.PersistenceService.Stores;

public class FileStoreTests : IClassFixture<ApplicationDbContextFixture>
{
    private readonly ApplicationDbContext _dbContext;

    public FileStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
    }

    [Fact]
    public async Task FileDDLMigration_ShouldHaveHappened()
    {
        int fileRows = await _dbContext.Files.CountAsync();
        Assert.Equal(0, fileRows);
    }

    [Fact]
    public async Task InsertTestFiles_ShouldInsertTestFiles()
    {
        int numFiles = 2;
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(
            c => c.AddRange(It.IsAny<IEnumerable<Models.File>>())
        );
        mockContext
            .Setup(c => c.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(numFiles);

        var fileStore = new FileStore(mockContext.Object);

        var result = await fileStore.InsertTestFiles(numFiles);

        Assert.NotNull(result);
        Assert.Equal(numFiles, result.Count);
        mockContext.Verify(
            c => c.AddRange(It.IsAny<IEnumerable<Models.File>>()),
            Times.Once
        );
        mockContext.Verify(
            c => c.SaveChangesAsync(default(CancellationToken)),
            Times.Once
        );
    }

    [Fact]
    public async Task InsertFiles_ShouldInsertFiles()
    {
        List<Models.File> files = new List<Models.File>();
        string testNamePrefix = "test-name-";
        string testKeyPrefix = "test-key-";
        for (int i = 0; i < 10; i++)
        {
            Models.File f = new Models.File
            {
                Name = testNamePrefix + i.ToString(),
                StoreKey = testKeyPrefix + i.ToString()
            };
            files.Add(f);
        }

        FileStore fileStore = new FileStore(_dbContext);
        int numFiles1 = await _dbContext.Files.CountAsync();
        int numInserted = await fileStore.InsertFiles(files);
        int numFiles2 = await _dbContext.Files.CountAsync();
        Assert.Equal(numFiles2 - numFiles1, numInserted);

        List<Models.File> loaded = _dbContext.Files
            .OrderByDescending(f => f.UploadedAt)
            .Take(10)
            .ToList();
        Assert.Equal(
            files.Select(f => f.Name),
            loaded.Select(f => f.Name).OrderBy(name => name)
        );
    }
}
