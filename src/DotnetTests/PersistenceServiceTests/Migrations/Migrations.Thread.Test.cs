using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using Thread = PersistenceService.Models.Thread;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ThreadMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ThreadMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(Thread))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(Thread.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(Thread.ChannelId)
        )!;
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdProperty.GetColumnType());
        Assert.False(channelIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(Thread.ConcurrencyStamp)
        )!;
        Assert.Equal("uuid", concurrencyStampProperty.GetColumnType());
        Assert.Equal(
            "gen_random_uuid()",
            concurrencyStampProperty.GetDefaultValueSql()
        );
    }

    [Fact]
    public void FirstMessageIdColumn()
    {
        var firstMessageIdProperty = _entityType.FindProperty(
            nameof(Thread.FirstMessageId)
        )!;
        var foreignKey = firstMessageIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", firstMessageIdProperty.GetColumnType());
        Assert.False(firstMessageIdProperty.IsColumnNullable());
    }

    [Fact]
    public void NumMessagesColumn()
    {
        var numMessagesProperty = _entityType.FindProperty(
            nameof(Thread.NumMessages)
        )!;
        Assert.Equal("2", numMessagesProperty.GetDefaultValueSql());
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(Thread.WorkspaceId)
        )!;
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", workspaceIdProperty.GetColumnType());
        Assert.False(workspaceIdProperty.IsColumnNullable());
    }

    [Fact]
    public async Task ThreadDDLMigration_ShouldHaveHappened()
    {
        int numThreadRows = await _dbContext.Threads.CountAsync();
        Assert.True(numThreadRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var firstMessageIdProperty = _entityType.FindProperty(
            nameof(Thread.FirstMessageId)
        )!;
        var firstMessageIdIndex = _entityType.FindIndex(firstMessageIdProperty);
        Assert.NotNull(firstMessageIdIndex);
        Assert.True(firstMessageIdIndex.IsUnique);
        var channelIdIndex = _entityType.FindIndex(
            _entityType.FindProperty(nameof(Thread.ChannelId))!
        );
        Assert.NotNull(channelIdIndex);
    }
}
