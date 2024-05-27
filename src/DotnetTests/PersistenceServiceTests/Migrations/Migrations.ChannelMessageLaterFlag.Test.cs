using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ChannelMessageLaterFlagMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageLaterFlagMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
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
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdProperty.GetColumnType());
        Assert.False(channelIdProperty.IsColumnNullable());
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
        var channelMessageIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.ChannelMessageId)
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
            nameof(ChannelMessageLaterFlag.CreatedAt)
        )!;
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.UserId)
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
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageLaterFlag.WorkspaceId)
        )!;
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", workspaceIdProperty.GetColumnType());
        Assert.False(workspaceIdProperty.IsColumnNullable());
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
