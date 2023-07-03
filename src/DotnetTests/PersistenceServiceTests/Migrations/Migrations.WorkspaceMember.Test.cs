using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class WorkspaceMemberMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public WorkspaceMemberMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
        var adminIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.Admin)
        )!;
        Assert.Equal(false, adminIdProperty.GetDefaultValue());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.AvatarId)
        )!;
        string avatarIdColumnType = avatarIdProperty.GetColumnType();
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdColumnType);
    }

    [Fact]
    public void JoinedAtColumn()
    {
        var joinedAtProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.JoinedAt)
        )!;
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
        Assert.False(notificationsSoundProperty.IsColumnNullable());
        Assert.Equal(0, notificationsSoundProperty.GetDefaultValue());
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
        Assert.Equal(false, ownerProperty.GetDefaultValue());
    }

    [Fact]
    public void ThemeIdColumn()
    {
        var themeIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.ThemeId)
        )!;
        string themeIdColumnType = themeIdProperty.GetColumnType();
        var foreignKey = themeIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", themeIdColumnType);
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
        string userIdPropertyColumnType = userIdProperty.GetColumnType();
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", userIdPropertyColumnType);
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(WorkspaceMember.WorkspaceId)
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
