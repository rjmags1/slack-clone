using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class WorkspaceInviteMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public WorkspaceInviteMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(WorkspaceInvite))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(WorkspaceInvite.Id))!;
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
            nameof(WorkspaceInvite.AdminId)
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
    public void WorkspaceInviteStatusColumn()
    {
        var workspaceInviteStatusProperty = _entityType.FindProperty(
            nameof(WorkspaceInvite.WorkspaceInviteStatus)
        )!;
        Assert.Equal("1", workspaceInviteStatusProperty.GetDefaultValueSql());
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(WorkspaceInvite.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(WorkspaceInvite.UserId)
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
            nameof(WorkspaceInvite.WorkspaceId)
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
    public async Task WorkspaceInviteDDLMigration_ShouldHaveHappened()
    {
        int numWorkspaceInviteRows =
            await _dbContext.WorkspaceInvites.CountAsync();
        Assert.True(numWorkspaceInviteRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(WorkspaceInvite.CreatedAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(createdAtProperty));
        var workspaceInviteStatusProperty = _entityType.FindProperty(
            nameof(WorkspaceInvite.WorkspaceInviteStatus)
        )!;
        Assert.NotNull(_entityType.FindIndex(workspaceInviteStatusProperty));
        var userIdIndex = _entityType.FindIndex(
            _entityType.FindProperty(nameof(WorkspaceInvite.UserId))!
        );
        Assert.NotNull(userIdIndex);
    }
}
