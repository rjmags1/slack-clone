using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class WorkspaceSearchMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public WorkspaceSearchMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(WorkspaceSearch))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(WorkspaceSearch.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(WorkspaceSearch.CreatedAt)
        )!;
        Assert.Equal("timestamp", createdAtProperty.GetColumnType());
        Assert.Equal("now()", createdAtProperty.GetDefaultValueSql());
    }

    [Fact]
    public void QueryColumn()
    {
        var queryProperty = _entityType.FindProperty(
            nameof(WorkspaceSearch.Query)
        )!;
        Assert.Equal(80, queryProperty.GetMaxLength());
        Assert.False(queryProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(WorkspaceSearch.UserId)
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
            nameof(WorkspaceSearch.WorkspaceId)
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
    public async Task WorkspaceSearchDDLMigration_ShouldHaveHappened()
    {
        int numWorkspaceSearchRows =
            await _dbContext.WorkspaceSearches.CountAsync();
        Assert.True(numWorkspaceSearchRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var createdAtProperty = _entityType.FindProperty(
            nameof(WorkspaceSearch.CreatedAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(createdAtProperty));
        var workspaceIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(WorkspaceSearch.WorkspaceId))!,
                _entityType.FindProperty(nameof(WorkspaceSearch.UserId))!,
            }
        )!;
        Assert.NotNull(workspaceIdUserIdIndex);
    }
}
