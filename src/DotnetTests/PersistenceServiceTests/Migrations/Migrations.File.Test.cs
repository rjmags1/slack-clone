using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using Models = PersistenceService.Models;
using System.ComponentModel;

namespace DotnetTests.PersistenceService.Migrations;

[Collection("Database collection")]
public class FileMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    public FileMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
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
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var idProperty = entityType.FindProperty(nameof(Models.File.Id))!;
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void NameColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var nameProperty = entityType.FindProperty(nameof(Models.File.Name))!;
        var maxLength = nameProperty.GetMaxLength();
        var nameColumnNullable = nameProperty.IsColumnNullable();
        Assert.Equal(maxLength, 80);
        Assert.False(nameColumnNullable);
    }

    [Fact]
    public void StoreKeyColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var storeKeyProperty = entityType.FindProperty(
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
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var uploadedAtProperty = entityType.FindProperty(
            nameof(Models.File.UploadedAt)
        )!;
        string uploadedAtColumnType = uploadedAtProperty.GetColumnType();
        string defaultValueSql = uploadedAtProperty.GetDefaultValueSql()!;
        var uploadedAtColumnNullable = uploadedAtProperty.IsColumnNullable();
        Assert.Equal("timestamp", uploadedAtColumnType);
        Assert.Equal("now()", defaultValueSql);
        Assert.False(uploadedAtColumnNullable);
    }

    [Fact]
    public void DirectMessageIdColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var directMessageIdProperty = entityType.FindProperty(
            nameof(Models.File.DirectMessageId)
        )!;
        string directMessageIdColumnType =
            directMessageIdProperty.GetColumnType();
        var foreignKey = entityType
            .FindProperty(nameof(Models.File.DirectMessageId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal(directMessageIdColumnType, "uuid");
    }

    [Fact]
    public void DirectMessageGroupIdColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var directMessageGroupIdProperty = entityType.FindProperty(
            nameof(Models.File.DirectMessageGroupId)
        )!;
        var directMessageGroupIdColumnType =
            directMessageGroupIdProperty.GetColumnType();
        var foreignKey = entityType
            .FindProperty(nameof(Models.File.DirectMessageGroupId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal(directMessageGroupIdColumnType, "uuid");
    }

    [Fact]
    public void ChannelMessageIdColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var channelMessageIdProperty = entityType.FindProperty(
            nameof(Models.File.ChannelMessageId)
        )!;
        var foreignKey = entityType
            .FindProperty(nameof(Models.File.ChannelMessageId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();
        string channelMessageIdColumnType =
            channelMessageIdProperty.GetColumnType();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal(channelMessageIdColumnType, "uuid");
    }

    [Fact]
    public void ChannelIdColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var channelIdProperty = entityType.FindProperty(
            nameof(Models.File.ChannelId)
        )!;
        string channelIdColumnType = channelIdProperty.GetColumnType();
        var foreignKey = entityType
            .FindProperty(nameof(Models.File.ChannelId))!
            .GetContainingForeignKeys()
            .SingleOrDefault();
        string channelMessageIdColumnType = channelIdProperty.GetColumnType();

        Assert.Equal(channelIdColumnType, "uuid");
    }

    [Fact]
    public void Indexes()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Models.File))!;
        var channelIdProperty = entityType.FindProperty(
            nameof(Models.File.ChannelId)
        )!;
        Assert.NotNull(channelIdProperty.GetIndex());
        var channelMessageIdProperty = entityType.FindProperty(
            nameof(Models.File.ChannelMessageId)
        )!;
        Assert.NotNull(channelMessageIdProperty.GetIndex());
        var directMessageIdProperty = entityType.FindProperty(
            nameof(Models.File.DirectMessageId)
        )!;
        Assert.NotNull(directMessageIdProperty.GetIndex());
        var directMessageGroupIdProperty = entityType.FindProperty(
            nameof(Models.File.DirectMessageGroupId)
        )!;
        Assert.NotNull(directMessageGroupIdProperty.GetIndex());
        var uploadedAtProperty = entityType.FindProperty(
            nameof(Models.File.UploadedAt)
        )!;
        Assert.NotNull(uploadedAtProperty.GetIndex());
    }
}
