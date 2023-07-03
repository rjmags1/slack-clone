using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class ChannelMessageLaterFlagMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageLaterFlagMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(ChannelMessageLaterFlag)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.Id)
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
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.ChannelId)
        )!;
        string avatarIdColumnType = channelIdProperty.GetColumnType();
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdColumnType);
    }

    [Fact]
    public void ChannelLaterFlagStatusColumn()
    {
        var channelInviteStatusProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.ChannelLaterFlagStatus)
        )!;
        Assert.Equal("1", channelInviteStatusProperty.GetDefaultValueSql());
    }

    [Fact]
    public void ChannelMessageIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.ChannelMessageId)
        )!;
        string avatarIdColumnType = channelIdProperty.GetColumnType();
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdColumnType);
    }

    [Fact]
    public void UserIdColumn()
    {
        var createdByIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.UserId)
        )!;
        string createdByIdPropertyColumnType =
            createdByIdProperty.GetColumnType();
        var foreignKey = createdByIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", createdByIdPropertyColumnType);
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.WorkspaceId)
        )!;
        string workspaceIdPropertyColumnType =
            workspaceIdProperty.GetColumnType();
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", workspaceIdPropertyColumnType);
    }

    [Fact]
    public async Task ChannelMessageLaterFlagDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageLaterFlagRows =
            await _dbContext.ChannelMessageLaterFlags.CountAsync();
        Assert.True(numChannelMessageLaterFlagRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var channelMessageIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(ChannelMessageLaterFlag.ChannelMessageId)
                )!,
                _entityType.FindProperty(
                    nameof(ChannelMessageLaterFlag.UserId)
                )!,
            }
        );
        Assert.NotNull(channelMessageIdUserIdIndex);
        Assert.True(channelMessageIdUserIdIndex.IsUnique);

        var workspaceIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(ChannelMessageLaterFlag.WorkspaceId)
                )!,
                _entityType.FindProperty(
                    nameof(ChannelMessageLaterFlag.UserId)
                )!,
            }
        );
        Assert.NotNull(workspaceIdUserIdIndex);
    }
}
