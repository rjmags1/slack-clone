using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class DirectMessageReplyMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageReplyMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageReply)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.Id)
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
    public void DirectMessageIdColumn()
    {
        var channelMessageIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.DirectMessageId)
        )!;
        string channelMessageIdColumnType =
            channelMessageIdProperty.GetColumnType();
        var foreignKey = channelMessageIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelMessageIdColumnType);
    }

    [Fact]
    public void MessageRepliedToIdColumn()
    {
        var messageRepliedToIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.MessageRepliedToId)
        )!;
        string messageRepliedToIdColumnType =
            messageRepliedToIdProperty.GetColumnType();
        var foreignKey = messageRepliedToIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", messageRepliedToIdColumnType);
    }

    [Fact]
    public void RepliedToIdColumn()
    {
        var repliedToIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.RepliedToId)
        )!;
        string repliedToIdColumnType = repliedToIdProperty.GetColumnType();
        var foreignKey = repliedToIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", repliedToIdColumnType);
    }

    [Fact]
    public void ReplierIdColumn()
    {
        var replierIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.ReplierId)
        )!;
        string replierIdColumnType = replierIdProperty.GetColumnType();
        var foreignKey = replierIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", replierIdColumnType);
    }

    [Fact]
    public async Task DirectMessageReplyDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageReplyRows =
            await _dbContext.DirectMessageReplies.CountAsync();
        Assert.True(numDirectMessageReplyRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var channelMessageIdIndex = _entityType.FindIndex(
            _entityType.FindProperty(
                nameof(DirectMessageReply.DirectMessageId)
            )!
        );
        Assert.NotNull(channelMessageIdIndex);
        Assert.True(channelMessageIdIndex.IsUnique);
        Assert.NotNull(
            _entityType.FindIndex(
                _entityType.FindProperty(
                    nameof(DirectMessageReply.MessageRepliedToId)
                )!
            )
        );
    }
}
