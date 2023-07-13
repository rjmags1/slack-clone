using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class DirectMessageMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public DirectMessageMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(DirectMessage))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(DirectMessage.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void ContentColumn()
    {
        var contentProperty = _entityType.FindProperty(
            nameof(DirectMessage.Content)
        )!;
        Assert.Equal(2500, contentProperty.GetMaxLength());
        Assert.False(contentProperty.IsColumnNullable());
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(DirectMessage.ConcurrencyStamp)
        )!;
        Assert.Equal("uuid", concurrencyStampProperty.GetColumnType());
        Assert.Equal(
            "gen_random_uuid()",
            concurrencyStampProperty.GetDefaultValueSql()
        );
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(DirectMessage.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DeletedColumn()
    {
        var deletedProperty = _entityType.FindProperty(
            nameof(DirectMessage.Deleted)
        )!;
        Assert.Equal("false", deletedProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DraftColumn()
    {
        var draftProperty = _entityType.FindProperty(
            nameof(DirectMessage.Draft)
        )!;
        Assert.Equal("true", draftProperty.GetDefaultValueSql());
    }

    [Fact]
    public void LastEditColumn()
    {
        var lastEditProperty = _entityType.FindProperty(
            nameof(DirectMessage.LastEdit)
        )!;
        Assert.Equal("timestamp", lastEditProperty.GetColumnType());
        Assert.True(lastEditProperty.IsColumnNullable());
    }

    [Fact]
    public void SentAtColumn()
    {
        var sentAtProperty = _entityType.FindProperty(
            nameof(DirectMessage.SentAt)
        )!;
        Assert.Equal("timestamp", sentAtProperty.GetColumnType());
        Assert.True(sentAtProperty.IsColumnNullable());
    }

    [Fact]
    public void UserIdColumn()
    {
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessage.UserId)
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
    public async Task DirectMessageDDLMigration_ShouldHaveHappened()
    {
        int numDirectMessageRows = await _dbContext.DirectMessages.CountAsync();
        Assert.True(numDirectMessageRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var directMessageIdProperty = _entityType.FindProperty(
            nameof(DirectMessage.DirectMessageGroupId)
        )!;
        Assert.NotNull(_entityType.FindIndex(directMessageIdProperty));
        var deletedProperty = _entityType.FindProperty(
            nameof(DirectMessage.Deleted)
        )!;
        Assert.NotNull(_entityType.FindIndex(deletedProperty));
        var draftProperty = _entityType.FindProperty(
            nameof(DirectMessage.Draft)
        )!;
        Assert.NotNull(_entityType.FindIndex(draftProperty));
        var sentAtProperty = _entityType.FindProperty(
            nameof(DirectMessage.SentAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(sentAtProperty));
        var userIdProperty = _entityType.FindProperty(
            nameof(DirectMessage.UserId)
        )!;
        Assert.NotNull(_entityType.FindIndex(userIdProperty));
    }
}
