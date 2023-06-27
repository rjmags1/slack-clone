using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace DotnetTests.PersistenceService.Migrations;

[Collection("Database collection")]
public class WorkspaceMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IEntityType _entityType;

    public WorkspaceMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(Workspace))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(User.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(nameof(User.AvatarId))!;
        string avatarIdColumnType = avatarIdProperty.GetColumnType();
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdColumnType);
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.ConcurrencyStamp)
        )!;
        Assert.Equal("uuid", concurrencyStampProperty.GetColumnType());
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DescriptionColumn()
    {
        var descriptionColumnProperty = _entityType.FindProperty(
            nameof(Workspace.Description)
        )!;
        Assert.Equal(
            "character varying(120)",
            descriptionColumnProperty.GetColumnType()
        );
        Assert.False(descriptionColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NameColumn()
    {
        var nameColumnProperty = _entityType.FindProperty(
            nameof(Workspace.Name)
        )!;
        Assert.Equal(
            "character varying(80)",
            nameColumnProperty.GetColumnType()
        );
        Assert.False(nameColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NumMembersColumn()
    {
        var numMembersColumnProperty = _entityType.FindProperty(
            nameof(Workspace.NumMembers)
        )!;
        Assert.Equal("1", numMembersColumnProperty.GetDefaultValueSql());
    }

    [Fact]
    public async Task WorkspaceDDLMigration_ShouldHaveHappened()
    {
        int numWorkspaceRows = await _dbContext.Workspaces.CountAsync();
        Assert.True(numWorkspaceRows >= 0);
    }
}
