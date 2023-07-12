using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ChannelMessageMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(ChannelMessage))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(ChannelMessage.Id))!;
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
            nameof(ChannelMessage.ChannelId)
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
    public void ContentColumn()
    {
        var contentProperty = _entityType.FindProperty(
            nameof(ChannelMessage.Content)
        )!;
        Assert.Equal(2500, contentProperty.GetMaxLength());
        Assert.False(contentProperty.IsColumnNullable());
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(ChannelMessage.ConcurrencyStamp)
        )!;
        Assert.Equal("uuid", concurrencyStampProperty.GetColumnType());
        Assert.Equal(
            "gen_random_uuid()",
            concurrencyStampProperty.GetDefaultValueSql()
        );
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(ChannelMessage.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DeletedColumn()
    {
        var deletedProperty = _entityType.FindProperty(
            nameof(ChannelMessage.Deleted)
        )!;
        Assert.Equal(false, deletedProperty.GetDefaultValue());
    }

    [Fact]
    public void DraftColumn()
    {
        var draftProperty = _entityType.FindProperty(
            nameof(ChannelMessage.Draft)
        )!;
        Assert.Equal("true", draftProperty.GetDefaultValueSql());
    }

    [Fact]
    public void LastEditColumn()
    {
        var lastEditProperty = _entityType.FindProperty(
            nameof(ChannelMessage.LastEdit)
        )!;
        Assert.Equal("timestamp", lastEditProperty.GetColumnType());
        Assert.True(lastEditProperty.IsColumnNullable());
    }

    [Fact]
    public void SentAtColumn()
    {
        var sentAtProperty = _entityType.FindProperty(
            nameof(ChannelMessage.SentAt)
        )!;
        Assert.Equal("timestamp", sentAtProperty.GetColumnType());
        Assert.True(sentAtProperty.IsColumnNullable());
    }

    [Fact]
    public void threadIdColumn()
    {
        var threadIdProperty = _entityType.FindProperty(
            nameof(ChannelMessage.ThreadId)
        )!;
        string threadIdPropertyColumnType = threadIdProperty.GetColumnType();
        var foreignKey = threadIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", threadIdPropertyColumnType);
        Assert.True(threadIdProperty.IsColumnNullable());
    }

    [Fact]
    public void userIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessage.UserId)
        )!;
        string userIdPropertyColumnType = userIdProperty.GetColumnType();
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", userIdPropertyColumnType);
        Assert.False(userIdProperty.IsColumnNullable());
    }

    [Fact]
    public async Task ChannelMessageDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageRows =
            await _dbContext.ChannelMessages.CountAsync();
        Assert.True(numChannelMessageRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelMessage.ChannelId)
        )!;
        Assert.NotNull(_entityType.FindIndex(channelIdProperty));
        var deletedProperty = _entityType.FindProperty(
            nameof(ChannelMessage.Deleted)
        )!;
        Assert.NotNull(_entityType.FindIndex(deletedProperty));
        var draftProperty = _entityType.FindProperty(
            nameof(ChannelMessage.Draft)
        )!;
        Assert.NotNull(_entityType.FindIndex(draftProperty));
        var sentAtProperty = _entityType.FindProperty(
            nameof(ChannelMessage.SentAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(sentAtProperty));
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessage.UserId)
        )!;
        Assert.NotNull(_entityType.FindIndex(userIdProperty));
    }
}
