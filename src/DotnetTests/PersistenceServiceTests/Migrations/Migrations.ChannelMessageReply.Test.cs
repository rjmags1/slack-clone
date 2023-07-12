using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ChannelMessageReplyMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageReplyMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(ChannelMessageReply)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.Id)
        )!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void ChannelMessageIdColumn()
    {
        var channelMessageIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.ChannelMessageId)
        )!;
        var foreignKey = channelMessageIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelMessageIdProperty.GetColumnType());
        Assert.False(channelMessageIdProperty.IsColumnNullable());
    }

    [Fact]
    public void MessageRepliedToIdColumn()
    {
        var messageRepliedToIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.MessageRepliedToId)
        )!;
        var foreignKey = messageRepliedToIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", messageRepliedToIdProperty.GetColumnType());
        Assert.False(messageRepliedToIdProperty.IsColumnNullable());
    }

    [Fact]
    public void RepliedToIdColumn()
    {
        var repliedToIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.RepliedToId)
        )!;
        var foreignKey = repliedToIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", repliedToIdProperty.GetColumnType());
        Assert.False(repliedToIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ReplierIdColumn()
    {
        var replierIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.ReplierId)
        )!;
        var foreignKey = replierIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", replierIdProperty.GetColumnType());
        Assert.False(replierIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ThreadIdColumn()
    {
        var threadIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReply.ThreadId)
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
    public async Task ChannelMessageReplyDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageReplyRows =
            await _dbContext.ChannelMessageReplies.CountAsync();
        Assert.True(numChannelMessageReplyRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var channelMessageIdIndex = _entityType.FindIndex(
            _entityType.FindProperty(
                nameof(ChannelMessageReply.ChannelMessageId)
            )!
        );
        Assert.NotNull(channelMessageIdIndex);
        Assert.True(channelMessageIdIndex.IsUnique);
        Assert.NotNull(
            _entityType.FindIndex(
                _entityType.FindProperty(
                    nameof(ChannelMessageReply.MessageRepliedToId)
                )!
            )
        );
        Assert.NotNull(
            _entityType.FindIndex(
                _entityType.FindProperty(nameof(ChannelMessageReply.ThreadId))!
            )
        );
    }
}
