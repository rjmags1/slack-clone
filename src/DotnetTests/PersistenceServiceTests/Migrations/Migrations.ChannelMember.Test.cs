using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class ChannelMemberMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMemberMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
        var adminIdProperty = _entityType.FindProperty(
            nameof(ChannelMember.Admin)
        )!;
        Assert.Equal(false, adminIdProperty.GetDefaultValue());
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(ChannelMember.ChannelId)
        )!;
        string channelIdColumnType = channelIdProperty.GetColumnType();
        var foreignKey = channelIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdColumnType);
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
        Assert.Equal(false, starredProperty.GetDefaultValue());
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
