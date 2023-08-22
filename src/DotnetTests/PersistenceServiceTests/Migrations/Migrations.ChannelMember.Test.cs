using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ChannelMemberMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMemberMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(ChannelMember))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(ChannelMember.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void AdminColumn()
    {
        var adminProperty = _entityType.FindProperty(
            nameof(ChannelMember.Admin)
        )!;
        Assert.Equal("false", adminProperty.GetDefaultValueSql());
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelMember.ChannelId)
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
    public void EnableNotificationsColumn()
    {
        var enableNotificationsProperty = _entityType.FindProperty(
            nameof(ChannelMember.EnableNotifications)
        )!;
        Assert.Equal("true", enableNotificationsProperty.GetDefaultValueSql());
    }

    [Fact]
    public void LastViewedAtColumn()
    {
        var lastViewedAtProperty = _entityType.FindProperty(
            nameof(ChannelMember.LastViewedAt)
        )!;
        Assert.Equal("timestamp", lastViewedAtProperty.GetColumnType());
        Assert.True(lastViewedAtProperty.IsColumnNullable());
    }

    [Fact]
    public void StarredColumn()
    {
        var starredProperty = _entityType.FindProperty(
            nameof(ChannelMember.Starred)
        )!;
        Assert.Equal("false", starredProperty.GetDefaultValueSql());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMember.UserId)
        )!;
        string createdByIdPropertyColumnType = userIdProperty.GetColumnType();
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", createdByIdPropertyColumnType);
        Assert.False(userIdProperty.IsColumnNullable());
    }

    [Fact]
    public async Task ChannelMemberDDLMigration_ShouldHaveHappened()
    {
        int numChannelMemberRows = await _dbContext.ChannelMembers.CountAsync();
        Assert.True(numChannelMemberRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMember.UserId)
        )!;
        Assert.NotNull(_entityType.FindIndex(userIdProperty));
        var channelIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(ChannelMember.ChannelId))!,
                _entityType.FindProperty(nameof(ChannelMember.UserId))!,
            }
        );
        Assert.NotNull(channelIdUserIdIndex);
        Assert.True(channelIdUserIdIndex.IsUnique);
    }
}
