using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class DirectMessageGroupMemberMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageGroupMemberMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(
            typeof(DirectMessageGroupMember)
        )!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(
            nameof(DirectMessageGroupMember.Id)
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
        var directMessageGroupIdProperty = _entityType.FindProperty(
            nameof(DirectMessageGroupMember.DirectMessageGroupId)
        )!;
        string directMessageGroupIdColumnType =
            directMessageGroupIdProperty.GetColumnType();
        var foreignKey = directMessageGroupIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", directMessageGroupIdColumnType);
    }

    [Fact]
    public void LastViewedGroupMessagesAtColumn()
    {
        var lastViewedGroupMessagesAtProperty = _entityType.FindProperty(
            nameof(DirectMessageGroupMember.LastViewedGroupMessagesAt)
        )!;
        Assert.Equal(
            "timestamp",
            lastViewedGroupMessagesAtProperty.GetColumnType()
        );
        Assert.True(lastViewedGroupMessagesAtProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessageGroupMember.UserId)
        )!;
        string userIdColumnType = userIdProperty.GetColumnType();
        var foreignKey = userIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", userIdColumnType);
    }

    [Fact]
    public async Task DirectMessageGroupMembersDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageGroupMemberRows =
            await _dbContext.DirectMessageGroupMembers.CountAsync();
        Assert.True(numDirectMessageGroupMemberRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var userIdDirectMessageGroupIdIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(
                    nameof(DirectMessageGroupMember.UserId)
                )!,
                _entityType.FindProperty(
                    nameof(DirectMessageGroupMember.DirectMessageGroupId)
                )!
            }
        );
        Assert.NotNull(userIdDirectMessageGroupIdIndex);
        Assert.True(userIdDirectMessageGroupIdIndex.IsUnique);
    }
}