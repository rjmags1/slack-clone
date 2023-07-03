using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using Thread = PersistenceService.Models.Thread;

[Collection("Database collection")]
public class ThreadMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ThreadMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
        string channelIdColumnType = channelIdProperty.GetColumnType();
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdColumnType);
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
        string firstMessageIdColumnType =
            firstMessageIdProperty.GetColumnType();
        var foreignKey = firstMessageIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", firstMessageIdColumnType);
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
        string createdByIdPropertyColumnType =
            workspaceIdProperty.GetColumnType();
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", createdByIdPropertyColumnType);
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
    }
}
