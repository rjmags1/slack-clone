using PersistenceService.Stores;
using PersistenceService.Data.ApplicationDb;
using Moq;
using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;

using Models = PersistenceService.Models;

namespace DotnetTests.PersistenceService.Stores;

[Collection("Database collection")]
public class FileStoreTests
{
    private readonly ApplicationDbContext _dbContext;

    public FileStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
    public async void InsertFiles_ShouldInsertFiles()
    {
        List<Models.File> files = new List<Models.File>();
        string testNamePrefix = "test-file-name-";
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

        List<Models.File> loaded = (await fileStore.InsertFiles(files))
            .OrderByDescending(f => f.UploadedAt)
            .Take(10)
            .ToList();
        Assert.Equal(
            files.Select(f => f.Name),
            loaded.Select(f => f.Name).OrderBy(name => name)
        );
        Assert.All(loaded, f => Assert.NotNull(f.Id));
        Assert.All(loaded, f => Assert.NotNull(f.UploadedAt));
    }
}
