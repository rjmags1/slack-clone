using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTests.PersistenceService.Stores;

[Collection("Database collection")]
public class ThemeStoreTests
{
    private readonly ApplicationDbContext _dbContext;

    public ThemeStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
    }

    [Fact]
    public async void InsertThemes_ShouldInsertThemes()
    {
        List<Theme> themes = new List<Theme>();
        string testNamePrefix = "test-theme-name-";
        for (int i = 0; i < 10; i++)
        {
            Theme f = new Theme { Name = testNamePrefix + i.ToString(), };
            themes.Add(f);
        }

        ThemeStore themeStore = new ThemeStore(_dbContext);
        List<Theme> loaded = (await themeStore.InsertThemes(themes))
            .OrderByDescending(f => f.Name)
            .Take(10)
            .ToList();
        Assert.Equal(
            themes.Select(f => f.Name),
            loaded.Select(f => f.Name).OrderBy(name => name)
        );
        Assert.All(loaded, t => Assert.NotEqual(t.Id, Guid.Empty));
    }

    [Fact]
    public async void InsertShippedThemes_ShouldInsertShippedThemes()
    {
        Theme mockDarkTheme = new Theme { Name = "Dark" };
        Theme mockNormalTheme = new Theme { Name = "Normal" };
        List<Theme> shippedThemes = new List<Theme>
        {
            mockDarkTheme,
            mockNormalTheme
        };
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(c => c.AddRange(shippedThemes));
        mockContext
            .Setup(c => c.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(shippedThemes.Count);

        var themeStore = new ThemeStore(mockContext.Object);

        var result = await themeStore.InsertShippedThemes();

        Assert.NotNull(result);
        Assert.Equal(
            shippedThemes
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .ToList<string>(),
            result.OrderBy(t => t.Name).Select(t => t.Name).ToList<string>()
        );
        mockContext.Verify(
            c => c.AddRange(It.IsAny<List<Theme>>()),
            Times.Once
        );
        mockContext.Verify(
            c => c.SaveChangesAsync(default(CancellationToken)),
            Times.Once
        );
    }

    [Fact]
    public async void InsertThemes_ShouldRaiseDbUpdateExceptionOnNonUniqueName()
    {
        Theme theme1 = new Theme { Name = "test-theme-name" };
        Theme theme2 = new Theme { Name = "test-theme-name" };
        List<Theme> themes = new List<Theme> { theme1, theme2 };
        ThemeStore themeStore = new ThemeStore(_dbContext);

        await Assert.ThrowsAsync<DbUpdateException>(
            async () => await themeStore.InsertThemes(themes)
        );
    }
}
