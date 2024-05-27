using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using Models = PersistenceService.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotnetTests.PersistenceService.Migrations;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class FileMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IEntityType _entityType;

    public FileMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
    }

    [Fact]
    public async Task FileDDLMigration_ShouldHaveHappened()
    {
        int numFileRows = await _dbContext.Files.CountAsync();
        Assert.True(numFileRows >= 0);
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(Models.File.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void NameColumn()
    {
        var nameProperty = _entityType.FindProperty(nameof(Models.File.Name))!;
        var maxLength = nameProperty.GetMaxLength();
        var nameColumnNullable = nameProperty.IsColumnNullable();
        Assert.Equal(maxLength, 80);
        Assert.False(nameColumnNullable);
    }

    [Fact]
    public void StoreKeyColumn()
    {
        var storeKeyProperty = _entityType.FindProperty(
            nameof(Models.File.StoreKey)
        )!;
        var maxLength = storeKeyProperty.GetMaxLength();
        var nameColumnNullable = storeKeyProperty.IsColumnNullable();
        Assert.Equal(maxLength, 256);
        Assert.False(nameColumnNullable);
    }

    [Fact]
    public void UploadedAtColumn()
    {
        var uploadedAtProperty = _entityType.FindProperty(
            nameof(Models.File.UploadedAt)
        )!;
        string uploadedAtColumnType = uploadedAtProperty.GetColumnType();
        string defaultValueSql = uploadedAtProperty.GetDefaultValueSql()!;
        Assert.Equal("timestamp", uploadedAtColumnType);
        Assert.Equal("now()", defaultValueSql);
    }

    [Fact]
    public void DirectMessageIdColumn()
    {
        var directMessageIdProperty = _entityType.FindProperty(
            nameof(Models.File.DirectMessageId)
        )!;
        var foreignKey = _entityType
            .FindProperty(nameof(Models.File.DirectMessageId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", directMessageIdProperty.GetColumnType());
        Assert.True(directMessageIdProperty.IsColumnNullable());
    }

    [Fact]
    public void DirectMessageGroupIdColumn()
    {
        var directMessageGroupIdProperty = _entityType.FindProperty(
            nameof(Models.File.DirectMessageGroupId)
        )!;
        var foreignKey = _entityType
            .FindProperty(nameof(Models.File.DirectMessageGroupId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", directMessageGroupIdProperty.GetColumnType());
        Assert.True(directMessageGroupIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ChannelMessageIdColumn()
    {
        var channelMessageIdProperty = _entityType.FindProperty(
            nameof(Models.File.ChannelMessageId)
        )!;
        var foreignKey = _entityType
            .FindProperty(nameof(Models.File.ChannelMessageId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelMessageIdProperty.GetColumnType());
        Assert.True(channelMessageIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(Models.File.ChannelId)
        )!;
        var foreignKey = _entityType
            .FindProperty(nameof(Models.File.ChannelId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", channelIdProperty.GetColumnType());
        Assert.True(channelIdProperty.IsColumnNullable());
    }

    [Fact]
    public void Indexes()
    {
        var channelIdProperty = _entityType.FindProperty(
            nameof(Models.File.ChannelId)
        )!;
        Assert.NotNull(_entityType.FindIndex(channelIdProperty));
        var channelMessageIdProperty = _entityType.FindProperty(
            nameof(Models.File.ChannelMessageId)
        )!;
        Assert.NotNull(_entityType.FindIndex(channelMessageIdProperty));
        var directMessageIdProperty = _entityType.FindProperty(
            nameof(Models.File.DirectMessageId)
        )!;
        Assert.NotNull(_entityType.FindIndex(directMessageIdProperty));
        var directMessageGroupIdProperty = _entityType.FindProperty(
            nameof(Models.File.DirectMessageGroupId)
        )!;
        Assert.NotNull(_entityType.FindIndex(directMessageGroupIdProperty));
        var uploadedAtProperty = _entityType.FindProperty(
            nameof(Models.File.UploadedAt)
        )!;
        Assert.NotNull(_entityType.FindIndex(uploadedAtProperty));
    }
}
