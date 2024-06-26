using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class DirectMessageMentionMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageMentionMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageMention)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageMention.Id)
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
            nameof(DirectMessageMention.DirectMessageId)
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
            nameof(DirectMessageMention.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void MentionedIdColumn()
    {
        var mentionedIdProperty = _entityType.FindProperty(
            nameof(DirectMessageMention.MentionedId)
        )!;
        var foreignKey = mentionedIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", mentionedIdProperty.GetColumnType());
        Assert.False(mentionedIdProperty.IsColumnNullable());
    }

    [Fact]
    public void MentionerIdColumn()
    {
        var mentionerIdProperty = _entityType.FindProperty(
            nameof(DirectMessageMention.MentionerId)
        )!;
        var foreignKey = mentionerIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", mentionerIdProperty.GetColumnType());
        Assert.False(mentionerIdProperty.IsColumnNullable());
    }

    [Fact]
    public async Task DirectMessageMentionDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageMentionRows =
            await _dbContext.DirectMessageMentions.CountAsync();
        Assert.True(numDirectMessageMentionRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var mentionedIdDirectMessageIdMentionerIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageMention.MentionedId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageMention.DirectMessageId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageMention.MentionerId)
                )!,
            }
        );
        Assert.NotNull(mentionedIdDirectMessageIdMentionerIdIndex);
        Assert.True(mentionedIdDirectMessageIdMentionerIdIndex.IsUnique);

        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(nameof(DirectMessageMention.CreatedAt))!
        );
        Assert.NotNull(createdAtIndex);
    }
}
