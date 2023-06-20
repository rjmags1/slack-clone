using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class ThemeStore
{
    private ApplicationDbContext _context;

    public ThemeStore(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> InsertThemes(List<Theme> themes)
    {
        _context.AddRange(themes);
        return await _context.SaveChangesAsync();
    }

    public async Task<List<Theme>> InsertShippedThemes()
    {
        List<Theme> themes = new List<Theme>();
        themes.Add(new Theme { Name = "Dark" });
        themes.Add(new Theme { Name = "Normal" });

        await InsertThemes(themes);

        return themes;
    }
}
