using System.Text;
using PersistenceService.Data.ApplicationDb;

namespace PersistenceService.Stores;

public class Store : IDisposable, IStore
{
    public static Random random { get; set; } = new Random();
    private static string _letters { get; set; } =
        Enumerable
            .Range('A', 26)
            .Concat(Enumerable.Range('a', 26))
            .Select(x => (char)x)
            .ToString()!;
    protected ApplicationDbContext _context { get; set; }

    public Store(ApplicationDbContext context)
    {
        _context = context;
    }

    public static string GenerateRandomString(int length)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(_letters.Length);
            builder.Append(_letters[index]);
        }

        return builder.ToString();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public interface IStore { }
