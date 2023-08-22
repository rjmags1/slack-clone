using PersistenceService.Stores;
using PersistenceService.Data.ApplicationDb;
using DotnetTests.Fixtures;

using Models = PersistenceService.Models;
using DotnetTests.PersistenceService.Utils;

namespace DotnetTests.PersistenceService.Stores;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class FileStoreTests1
{
    private readonly ApplicationDbContext _dbContext;
    private readonly FileStore _fileStore;

    public FileStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _fileStore = new FileStore(_dbContext);
    }

    [Fact]
    public async void InsertFiles_ShouldInsertFiles()
    {
        List<Models.File> files = new List<Models.File>();
        for (int i = 0; i < 10; i++)
        {
            files.Add(StoreTestUtils.CreateTestFileRecord());
        }

        FileStore fileStore = new(_dbContext);

        List<Models.File> loaded = await fileStore.InsertFiles(files);
        Assert.Equal(
            files.Select(f => f.Name).OrderBy(name => name),
            loaded.Select(f => f.Name).OrderBy(name => name)
        );
        Assert.All(loaded, f => Assert.NotEqual(f.Id, Guid.Empty));
        Assert.All(
            loaded,
            f => Assert.NotEqual(f.UploadedAt, default(DateTime))
        );
    }
}
