using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class WorkspaceAdminPermissionsMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public WorkspaceAdminPermissionsMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(WorkspaceAdminPermissions)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(WorkspaceAdminPermissions.Id)
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
    public void AdminIdColumn()
    {
        var adminIdProperty = _entityType.FindProperty(
            nameof(WorkspaceAdminPermissions.AdminId)
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
    public void ConcurrencyStampColumn()
    {
        var workspaceInviteStatusProperty = _entityType.FindProperty(
            nameof(WorkspaceAdminPermissions.ConcurrencyStamp)
        )!;
        Assert.Equal(
            "gen_random_uuid()",
            workspaceInviteStatusProperty.GetDefaultValueSql()
        );
    }

    [Fact]
    public void WorkspaceAdminPermissionsMaskColumn()
    {
        var maskProperty = _entityType.FindProperty(
            nameof(WorkspaceAdminPermissions.WorkspaceAdminPermissionsMask)
        )!;
        Assert.Equal("1", maskProperty.GetDefaultValueSql());
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(WorkspaceAdminPermissions.WorkspaceId)
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
    public async Task WorkspaceAdminPermissionsDDLMigration_ShouldHaveHappened()
    {
        int numWorkspaceAdminPermissionsRows =
            await _dbContext.WorkspaceAdminPermissions.CountAsync();
        Assert.True(numWorkspaceAdminPermissionsRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var adminIdWorkspaceIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(WorkspaceAdminPermissions.AdminId)
                )!,
                _entityType.FindProperty(
                    nameof(WorkspaceAdminPermissions.WorkspaceId)
                )!
            }
        );
        Assert.NotNull(adminIdWorkspaceIdIndex);
        Assert.True(adminIdWorkspaceIdIndex.IsUnique);
    }
}
