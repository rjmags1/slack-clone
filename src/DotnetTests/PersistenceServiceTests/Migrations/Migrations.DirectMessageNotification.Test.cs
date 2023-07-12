using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class DirectMessageNotificationMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageNotificationMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageNotification)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageNotification.Id)
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
            nameof(DirectMessageNotification.DirectMessageId)
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
            nameof(DirectMessageNotification.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DirectMessageNotificationTypeColumn()
    {
        var notifTypeProperty = _entityType.FindProperty(
            nameof(DirectMessageNotification.DirectMessageNotificationType)
        )!;
        Assert.False(notifTypeProperty.IsColumnNullable());
        Assert.Equal("integer", notifTypeProperty.GetColumnType());
    }

    [Fact]
    public void SeenColumn()
    {
        var seenProperty = _entityType.FindProperty(
            nameof(DirectMessageNotification.Seen)
        )!;
        Assert.Equal("false", seenProperty.GetDefaultValueSql());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessageNotification.UserId)
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
    public async Task DirectMessageNotificationDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageNotificationRows =
            await _dbContext.DirectMessageNotifications.CountAsync();
        Assert.True(numDirectMessageNotificationRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdDirectMessageIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageNotification.UserId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageNotification.DirectMessageId)
                )!,
            }
        );
        Assert.NotNull(userIdDirectMessageIdIndex);
        Assert.True(userIdDirectMessageIdIndex.IsUnique);

        var createdAtIndex = _entityType.FindIndex(
            _entityType.FindProperty(
                nameof(DirectMessageNotification.CreatedAt)
            )!
        );
        Assert.NotNull(createdAtIndex);
    }
}
