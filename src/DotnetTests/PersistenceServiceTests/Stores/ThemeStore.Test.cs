using DotnetTests.Fixtures;
using DotnetTests.PersistenceService.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTests.PersistenceService.Stores;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class ThemeStoreTests1
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ThemeStore _themeStore;

    public ThemeStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _themeStore = new ThemeStore(_dbContext);
    }

    [Fact]
    public async void InsertThemes_ShouldInsertThemes()
    {
        List<Theme> themes = new();
        for (int i = 0; i < 10; i++)
        {
            themes.Add(StoreTestUtils.CreateTestTheme());
        }

        List<Theme> loaded = await _themeStore.InsertThemes(themes);
        Assert.Equal(
            themes.Select(f => f.Name).OrderBy(name => name),
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
        Theme theme1 = StoreTestUtils.CreateTestTheme();
        Theme theme2 = StoreTestUtils.CreateTestTheme();
        theme2.Name = theme1.Name;
        List<Theme> themes = new List<Theme> { theme1, theme2 };

        await Assert.ThrowsAsync<DbUpdateException>(
            async () => await _themeStore.InsertThemes(themes)
        );
    }
}

[Trait("Category", "Order 2")]
[Collection("Filled Database Test Collection")]
public class ThemeStoreTests2
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ThemeStore _themeStore;

    public ThemeStoreTests2(
        FilledApplicationDbContextFixture filledApplicationDbContextFixture
    )
    {
        _dbContext = filledApplicationDbContextFixture.Context;
        _themeStore = new ThemeStore(_dbContext);
    }

    [Fact]
    public void SeedHappened()
    {
        Assert.Equal(2, _dbContext.Themes.Count());
    }
}
