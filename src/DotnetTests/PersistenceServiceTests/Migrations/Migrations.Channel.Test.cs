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
        Assert.Equal(defaultValueSql, "gen_random_uuid()");
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
        Assert.Equal(allowThreadsProperty.GetColumnType(), "boolean");
        Assert.False(allowThreadsProperty.IsColumnNullable());
    }

    [Fact]
    public void AllowedChannelPostersMask()
    {
        var allowedChannelPostersMask = _entityType.FindProperty(
            nameof(Channel.AllowedChannelPostersMask)
        )!;
        Assert.Equal(allowedChannelPostersMask.GetColumnType(), "integer");
        Assert.False(allowedChannelPostersMask.IsColumnNullable());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(
            nameof(Channel.AvatarId)
        )!;
        string avatarIdColumnType = avatarIdProperty.GetColumnType();
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal(avatarIdColumnType, "uuid");
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(Channel.ConcurrencyStamp)
        )!;
        Assert.Equal(concurrencyStampProperty.GetColumnType(), "uuid");
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(Channel.CreatedAt)
        )!;
        Assert.Equal(concurrencyStampProperty.GetColumnType(), "timestamp");
        Assert.Equal(concurrencyStampProperty.GetDefaultValueSql(), "now()");
    }

    [Fact]
    public void CreatedByIdColumn()
    {
        var createdByIdProperty = _entityType.FindProperty(
            nameof(Channel.CreatedById)
        )!;
        string createdByIdPropertyColumnType =
            createdByIdProperty.GetColumnType();
        var foreignKey = createdByIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal(createdByIdPropertyColumnType, "uuid");
    }

    [Fact]
    public void DescriptionColumn()
    {
        var descriptionColumnProperty = _entityType.FindProperty(
            nameof(Channel.Description)
        )!;
        Assert.Equal(
            descriptionColumnProperty.GetColumnType(),
            "character varying(120)"
        );
        Assert.False(descriptionColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NameColumn()
    {
        var nameColumnProperty = _entityType.FindProperty(
            nameof(Channel.Name)
        )!;
        Assert.Equal(
            nameColumnProperty.GetColumnType(),
            "character varying(40)"
        );
        Assert.False(nameColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void NumMembersColumn()
    {
        var numMembersColumnProperty = _entityType.FindProperty(
            nameof(Channel.NumMembers)
        )!;
        Assert.Equal(numMembersColumnProperty.GetDefaultValueSql(), "1");
    }

    [Fact]
    public void PrivateColumn()
    {
        var privateColumnProperty = _entityType.FindProperty(
            nameof(Channel.Private)
        )!;
        Assert.Equal(privateColumnProperty.GetColumnType(), "boolean");
        Assert.False(privateColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void TopicColumn()
    {
        var topicColumnProperty = _entityType.FindProperty(
            nameof(Channel.Topic)
        )!;
        Assert.Equal(
            topicColumnProperty.GetColumnType(),
            "character varying(40)"
        );
        Assert.False(topicColumnProperty.IsColumnNullable());
    }

    [Fact]
    public void WorkspaceIdColumn()
    {
        var workspaceIdProperty = _entityType.FindProperty(
            nameof(Channel.WorkspaceId)
        )!;
        string createdByIdPropertyColumnType =
            workspaceIdProperty.GetColumnType();
        var foreignKey = workspaceIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
        Assert.Equal(createdByIdPropertyColumnType, "uuid");
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
        Assert.NotNull(privateProperty.GetIndex());
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
