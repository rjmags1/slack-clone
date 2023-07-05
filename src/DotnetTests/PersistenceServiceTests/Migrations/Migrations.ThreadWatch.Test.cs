using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class ThreadWatchMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ThreadWatchMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(ThreadWatch))!;
    }

    [Fact]
    public void ThreadIdColumn()
    {
        var threadIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.ThreadId)
        )!;
        string channelIdColumnType = threadIdProperty.GetColumnType();
        var foreignKey = threadIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdColumnType);
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.UserId)
        )!;
        string channelIdColumnType = userIdProperty.GetColumnType();
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdColumnType);
    }

    [Fact]
    public async Task ThreadWatchDDLMigration_ShouldHaveHappened()
    {
        int numThreadWatchRows = await _dbContext.ThreadWatches.CountAsync();
        Assert.True(numThreadWatchRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.UserId)
        )!;
        var threadIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.ThreadId)
        )!;
        Assert.NotNull(_entityType.FindIndex(threadIdProperty));

        var userIdThreadIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty> { userIdProperty, threadIdProperty }
        );
        Assert.NotNull(userIdThreadIdIndex);
        Assert.True(userIdThreadIdIndex.IsUnique);
    }
}
