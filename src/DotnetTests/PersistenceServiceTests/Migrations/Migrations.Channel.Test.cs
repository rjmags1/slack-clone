using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

[Collection("Database collection")]
public class ChannelMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEntityType _entityType;

    public ChannelMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(Channel))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(Channel.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void AllowThreadsColumn()
    {
        var allowThreadsProperty = _entityType.FindProperty(
            nameof(Channel.AllowThreads)
        )!;
        Assert.Equal("true", allowThreadsProperty.GetDefaultValueSql());
    }

    [Fact]
    public void AllowedChannelPostersMask()
    {
        var allowedChannelPostersMask = _entityType.FindProperty(
            nameof(Channel.AllowedChannelPostersMask)
        )!;
        Assert.Equal("1", allowedChannelPostersMask.GetDefaultValueSql());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(
            nameof(Channel.AvatarId)
        )!;
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdProperty.GetColumnType());
        Assert.True(avatarIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(Channel.ConcurrencyStamp)
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
            nameof(Channel.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void CreatedByIdColumn()
    {
        var createdByIdProperty = _entityType.FindProperty(
            nameof(Channel.CreatedById)
        )!;
        var foreignKey = createdByIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", createdByIdProperty.GetColumnType());
        Assert.False(createdByIdProperty.IsColumnNullable());
    }

    [Fact]
    public void DescriptionColumn()
    {
        var descriptionColumnProperty = _entityType.FindProperty(
            nameof(Channel.Description)
        )!;
        Assert.Equal(
            "character varying(120)",
            descriptionColumnProperty.GetColumnType()
        );
        Assert.True(descriptionColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NameColumn()
    {
        var nameColumnProperty = _entityType.FindProperty(
            nameof(Channel.Name)
        )!;
        Assert.Equal(
            "character varying(40)",
            nameColumnProperty.GetColumnType()
        );
        Assert.False(nameColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NumMembersColumn()
    {
        var numMembersColumnProperty = _entityType.FindProperty(
            nameof(Channel.NumMembers)
        )!;
        Assert.Equal("1", numMembersColumnProperty.GetDefaultValueSql());
    }

    [Fact]
    public void PrivateColumn()
    {
        var privateColumnProperty = _entityType.FindProperty(
            nameof(Channel.Private)
        )!;
        Assert.Equal("false", privateColumnProperty.GetDefaultValueSql());
    }

    [Fact]
    public void TopicColumn()
    {
        var topicColumnProperty = _entityType.FindProperty(
            nameof(Channel.Topic)
        )!;
        Assert.Equal(
            "character varying(40)",
            topicColumnProperty.GetColumnType()
        );
        Assert.True(topicColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(Channel.WorkspaceId)
        )!;
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", workspaceIdProperty.GetColumnType());
    }

    [Fact]
    public async Task ChannelDDLMigration_ShouldHaveHappened()
    {
        int numChannelRows = await _dbContext.Channels.CountAsync();
        Assert.True(numChannelRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var privateProperty = _entityType.FindProperty(
            nameof(Channel.Private)
        )!;
        Assert.NotNull(_entityType.FindIndex(privateProperty));
        var workspaceNameIndex = _entityType.FindIndex(
            new List<IReadOnlyProperty>
            {
                _entityType.FindProperty(nameof(Channel.WorkspaceId))!,
                _entityType.FindProperty(nameof(Channel.Name))!,
            }
        );
        Assert.NotNull(workspaceNameIndex);
        Assert.True(workspaceNameIndex.IsUnique);
    }
}
