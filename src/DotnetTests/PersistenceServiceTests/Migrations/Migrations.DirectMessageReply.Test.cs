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
        var directMessageIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.DirectMessageId)
        )!;
        var foreignKey = directMessageIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", directMessageIdProperty.GetColumnType());
        Assert.False(directMessageIdProperty.IsColumnNullable());
    }

    [Fact]
    public void MessageRepliedToIdColumn()
    {
        var messageRepliedToIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReply.MessageRepliedToId)
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
            nameof(DirectMessageReply.RepliedToId)
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
            nameof(DirectMessageReply.ReplierId)
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
