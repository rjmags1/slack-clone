using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ChannelMessageReactionMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageReactionMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(ChannelMessageReaction)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(ChannelMessageReaction.Id)
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
            nameof(ChannelMessageReaction.ChannelMessageId)
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
    public void CreatedAtColumn()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(ChannelMessageReaction.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void EmojiColumn()
    {
        var emojiProperty = _entityType.FindProperty(
            nameof(ChannelMessageReaction.Emoji)
        )!;
        Assert.Equal(4, emojiProperty.GetMaxLength());
        Assert.False(emojiProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageReaction.UserId)
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
    public async Task ChannelMessageReactionDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageReactionRows =
            await _dbContext.ChannelMessageReactions.CountAsync();
        Assert.True(numChannelMessageReactionRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageNotification.UserId)
        )!;
        Assert.NotNull(_entityType.FindIndex(userIdProperty));
        var channelMessageIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(ChannelMessageNotification.ChannelMessageId)
                )!,
                userIdProperty
            }
        );
        Assert.NotNull(channelMessageIdUserIdIndex);
        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(
                nameof(ChannelMessageNotification.CreatedAt)
            )!
        );
        Assert.NotNull(createdAtIndex);
    }
}
