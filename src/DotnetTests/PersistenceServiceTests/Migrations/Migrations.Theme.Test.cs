using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace DotnetTests.PersistenceService.Migrations;

[Collection("Database collection")]
public class ThemeMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    public ThemeMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
    }

    [Fact]
    public void IdColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Theme))!;
        var idProperty = entityType.FindProperty(nameof(Theme.Id))!;
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void NameColumn()
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(Theme))!;
        var nameProperty = entityType.FindProperty(nameof(Theme.Name))!;
        var maxLength = nameProperty.GetMaxLength();
        var namePropertyUnique = nameProperty.IsUniqueIndex();
        Assert.Equal(maxLength, 40);
        Assert.True(namePropertyUnique);
    }

    [Fact]
    public async void ThemeDDLMigration_ShouldHaveHappened()
    {
        int numThemeRows = await _dbContext.Themes.CountAsync();
        Assert.True(numThemeRows >= 0);
    }
}
