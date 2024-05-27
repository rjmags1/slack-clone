using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class DirectMessageLaterFlagMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageLaterFlagMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageLaterFlag)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageLaterFlag.Id)
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
    public void DirectMessageGroupIdColumn()
    {
        var directMessageGroupId = _entityType.FindProperty(
            nameof(DirectMessageLaterFlag.DirectMessageGroupId)
        )!;
        var foreignKey = directMessageGroupId
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", directMessageGroupId.GetColumnType());
        Assert.False(directMessageGroupId.IsColumnNullable());
    }

    [Fact]
    public void DirectMessageLaterFlagStatusColumn()
    {
        var laterFlagStatusProperty = _entityType.FindProperty(
            nameof(DirectMessageLaterFlag.DirectMessageLaterFlagStatus)
        )!;
        Assert.Equal("1", laterFlagStatusProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DirectMessageIdColumn()
    {
        var directMessageIdProperty = _entityType.FindProperty(
            nameof(DirectMessageLaterFlag.DirectMessageId)
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
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessageLaterFlag.UserId)
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
            nameof(DirectMessageLaterFlag.WorkspaceId)
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
    public async Task DirectMessageLaterFlagDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageLaterFlagRows =
            await _dbContext.DirectMessageLaterFlags.CountAsync();
        Assert.True(numDirectMessageLaterFlagRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var directMessageIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageLaterFlag.DirectMessageId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageLaterFlag.UserId)
                )!,
            }
        );
        Assert.NotNull(directMessageIdUserIdIndex);
        Assert.True(directMessageIdUserIdIndex.IsUnique);

        var workspaceIdUserIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageLaterFlag.WorkspaceId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageLaterFlag.UserId)
                )!,
            }
        );
        Assert.NotNull(workspaceIdUserIdIndex);
    }
}
