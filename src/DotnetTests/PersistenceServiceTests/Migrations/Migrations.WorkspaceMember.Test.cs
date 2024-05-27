using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class WorkspaceMemberMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public WorkspaceMemberMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(WorkspaceMember))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(WorkspaceMember.Id))!;
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
            nameof(WorkspaceMember.Admin)
        )!;
        Assert.Equal("false", adminProperty.GetDefaultValueSql());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.AvatarId)
        )!;
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdProperty.GetColumnType());
        Assert.True(avatarIdProperty.IsColumnNullable());
    }

    [Fact]
    public void JoinedAtColumn()
    {
        var joinedAtProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.JoinedAt)
        )!;
        Assert.Equal("timestamp", joinedAtProperty.GetColumnType());
        Assert.Equal("now()", joinedAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void NotificationsAllowTimeStartColumn()
    {
        var notificationsAllowTimeStartProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.NotificationsAllowTimeStart)
        )!;
        Assert.True(notificationsAllowTimeStartProperty.IsColumnNullable());
        Assert.Equal(
            "time without time zone",
            notificationsAllowTimeStartProperty.GetColumnType()
        );
    }

    [Fact]
    public void NotificationsAllowTimeEndColumn()
    {
        var notificationsAllowTimeEndProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.NotificationsAllTimeEnd)
        )!;
        Assert.True(notificationsAllowTimeEndProperty.IsColumnNullable());
        Assert.Equal(
            "time without time zone",
            notificationsAllowTimeEndProperty.GetColumnType()
        );
    }

    [Fact]
    public void NotificationsSoundColumn()
    {
        var notificationsSoundProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.NotificationSound)
        )!;
        Assert.Equal("0", notificationsSoundProperty.GetDefaultValueSql());
    }

    [Fact]
    public void OnlineStatusUntilColumn()
    {
        var onlineStatusUntilProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.OnlineStatusUntil)
        )!;
        Assert.Equal("timestamp", onlineStatusUntilProperty.GetColumnType());
        Assert.True(onlineStatusUntilProperty.IsColumnNullable());
    }

    [Fact]
    public void OnlineStatusColumn()
    {
        var onlineStatusUntilProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.OnlineStatus)
        )!;
        Assert.True(onlineStatusUntilProperty.IsColumnNullable());
        Assert.Equal(20, onlineStatusUntilProperty.GetMaxLength());
    }

    [Fact]
    public void OwnerColumn()
    {
        var ownerProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.Owner)
        )!;
        Assert.Equal("false", ownerProperty.GetDefaultValueSql());
    }

    [Fact]
    public void ThemeIdColumn()
    {
        var themeIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.ThemeId)
        )!;
        var foreignKey = themeIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", themeIdProperty.GetColumnType());
        Assert.True(themeIdProperty.IsColumnNullable());
    }

    [Fact]
    public void TitleColumn()
    {
        var titleProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.Title)
        )!;
        Assert.Equal(80, titleProperty.GetMaxLength());
        Assert.False(titleProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.UserId)
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
            nameof(WorkspaceMember.WorkspaceId)
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
    public async Task WorkspaceMemberDDLMigration_ShouldHaveHappened()
    {
        int numWorkspaceMemberRows =
            await _dbContext.WorkspaceMembers.CountAsync();
        Assert.True(numWorkspaceMemberRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var joinedAtProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.JoinedAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(joinedAtProperty));
        var userIdWorkspaceIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(WorkspaceMember.UserId))!,
                _entityType.FindProperty(nameof(WorkspaceMember.WorkspaceId))!,
            }
        );
        Assert.NotNull(userIdWorkspaceIdIndex);
        Assert.True(userIdWorkspaceIdIndex.IsUnique);
        var workspaceIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(WorkspaceMember.WorkspaceId))!,
                _entityType.FindProperty(nameof(WorkspaceMember.UserId))!,
            }
        );
        Assert.NotNull(workspaceIdUserIdIndex);
    }
}
