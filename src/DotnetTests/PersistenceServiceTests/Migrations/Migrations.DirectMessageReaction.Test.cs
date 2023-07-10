using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class DirectMessageReactionMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageReactionMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageReaction)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageReaction.Id)
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
            nameof(DirectMessageReaction.DirectMessageId)
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
    public void CreatedAtColumn()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(DirectMessageReaction.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void EmojiColumn()
    {
        var emojiProperty = _entityType.FindProperty(
            nameof(DirectMessageReaction.Emoji)
        )!;
        Assert.Equal(4, emojiProperty.GetMaxLength());
        Assert.False(emojiProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReaction.UserId)
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
    public async Task DirectMessageReactionDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageReactionRows =
            await _dbContext.DirectMessageReactions.CountAsync();
        Assert.True(numDirectMessageReactionRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessageReaction.UserId)
        )!;
        Assert.NotNull(_entityType.FindIndex(userIdProperty));
        var directMessageIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageReaction.DirectMessageId)
                )!,
                userIdProperty
            }
        );
        Assert.NotNull(directMessageIdUserIdIndex);
        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(nameof(DirectMessageReaction.CreatedAt))!
        );
        Assert.NotNull(createdAtIndex);
    }
}
