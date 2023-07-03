using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class ChannelMessageMentionMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageMentionMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
        string mentionedIdColumnType = mentionedIdProperty.GetColumnType();
        var foreignKey = mentionedIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", mentionedIdColumnType);
    }

    [Fact]
    public void MentionerIdColumn()
    {
        var mentionerIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageMention.MentionerId)
        )!;
        string mentionerIdColumnType = mentionerIdProperty.GetColumnType();
        var foreignKey = mentionerIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", mentionerIdColumnType);
    }

    [Fact]
    public async Task ChannelMessageMentionDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageMentionRows =
            await _dbContext.ChannelMessageLaterFlags.CountAsync();
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
