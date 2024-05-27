using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ChannelMessageMentionMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageMentionMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(ChannelMessageMention)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(ChannelMessageMention.Id)
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
            nameof(ChannelMessageMention.ChannelMessageId)
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
            nameof(ChannelMessageMention.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void MentionedIdColumn()
    {
        var mentionedIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageMention.MentionedId)
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
            nameof(ChannelMessageMention.MentionerId)
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
    public async Task ChannelMessageMentionDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageMentionRows =
            await _dbContext.ChannelMessageMentions.CountAsync();
        Assert.True(numChannelMessageMentionRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var mentionedIdChannelMessageIdMentionerIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(ChannelMessageMention.MentionedId)
                )!,
                _entityType.FindProperty(
                    nameof(ChannelMessageMention.ChannelMessageId)
                )!,
                _entityType.FindProperty(
                    nameof(ChannelMessageMention.MentionerId)
                )!,
            }
        );
        Assert.NotNull(mentionedIdChannelMessageIdMentionerIdIndex);
        Assert.True(mentionedIdChannelMessageIdMentionerIdIndex.IsUnique);

        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(nameof(ChannelMessageMention.CreatedAt))!
        );
        Assert.NotNull(createdAtIndex);
    }
}
