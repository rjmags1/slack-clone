using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ChannelInviteMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelInviteMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(ChannelInvite))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(ChannelInvite.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void AdminIdColumn()
    {
        var adminIdProperty = _entityType.FindProperty(
            nameof(ChannelInvite.AdminId)
        )!;
        var foreignKey = adminIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", adminIdProperty.GetColumnType());
        Assert.False(adminIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelInvite.ChannelId)
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
    public void ChannelInviteStatusColumn()
    {
        var channelInviteStatusProperty = _entityType.FindProperty(
            nameof(ChannelInvite.ChannelInviteStatus)
        )!;
        Assert.Equal("1", channelInviteStatusProperty.GetDefaultValueSql());
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(Channel.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelInvite.UserId)
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
            nameof(ChannelInvite.WorkspaceId)
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
    public async Task ChannelInviteDDLMigration_ShouldHaveHappened()
    {
        int numChannelInviteRows = await _dbContext.ChannelInvites.CountAsync();
        Assert.True(numChannelInviteRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(ChannelInvite.CreatedAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(createdAtProperty));
        var channelInviteStatusProperty = _entityType.FindProperty(
            nameof(ChannelInvite.ChannelInviteStatus)
        )!;
        Assert.NotNull(_entityType.FindIndex(channelInviteStatusProperty));
        var userIdWorkspaceIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(ChannelInvite.UserId))!,
                _entityType.FindProperty(nameof(ChannelInvite.WorkspaceId))!,
            }
        );
        Assert.NotNull(userIdWorkspaceIdIndex);
    }
}
