using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ThreadWatchMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ThreadWatchMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(ThreadWatch))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(ThreadWatch.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void ThreadIdColumn()
    {
        var threadIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.ThreadId)
        )!;
        var foreignKey = threadIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", threadIdProperty.GetColumnType());
        Assert.False(threadIdProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ThreadWatch.UserId)
        )!;
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", userIdProperty.GetColumnType());
        Assert.False(userIdProperty.IsColumnNullable());
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
