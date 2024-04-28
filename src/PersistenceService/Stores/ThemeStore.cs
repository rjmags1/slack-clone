using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class ThemeStore : Store
{
    public ThemeStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<List<Theme>> InsertThemes(List<Theme> themes)
    {
        _context.AddRange(themes);
        await _context.SaveChangesAsync();
        return themes;
    }

    public async Task<List<Theme>> InsertShippedThemes()
    {
        List<Theme> themes =
            new()
            {
                new Theme { Name = "Dark" },
                new Theme { Name = "Normal" }
            };

        await InsertThemes(themes);

        return themes;
    }
}
