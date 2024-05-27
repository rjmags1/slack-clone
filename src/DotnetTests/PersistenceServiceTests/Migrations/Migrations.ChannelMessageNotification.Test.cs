using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ChannelMessageNotificationMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMessageNotificationMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(ChannelMessageNotification)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(ChannelMessageNotification.Id)
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
            nameof(ChannelMessageNotification.ChannelMessageId)
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
            nameof(ChannelMessageNotification.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void ChannelMessageNotificationTypeColumn()
    {
        var notifTypeProperty = _entityType.FindProperty(
            nameof(ChannelMessageNotification.ChannelMessageNotificationType)
        )!;
        Assert.False(notifTypeProperty.IsColumnNullable());
        Assert.Equal("integer", notifTypeProperty.GetColumnType());
    }

    [Fact]
    public void SeenColumn()
    {
        var seenProperty = _entityType.FindProperty(
            nameof(ChannelMessageNotification.Seen)
        )!;
        Assert.Equal("false", seenProperty.GetDefaultValueSql());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(ChannelMessageNotification.UserId)
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
    public async Task ChannelMessageNotificationDDLMigration_ShouldHaveHappened()
    {
        int numChannelMessageNotificationRows =
            await _dbContext.ChannelMessageNotifications.CountAsync();
        Assert.True(numChannelMessageNotificationRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdChannelMessageIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(ChannelMessageNotification.UserId)
                )!,
                _entityType.FindProperty(
                    nameof(ChannelMessageNotification.ChannelMessageId)
                )!,
            }
        );
        Assert.NotNull(userIdChannelMessageIdIndex);
        Assert.True(userIdChannelMessageIdIndex.IsUnique);

        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(
                nameof(ChannelMessageNotification.CreatedAt)
            )!
        );
        Assert.NotNull(createdAtIndex);
    }
}
