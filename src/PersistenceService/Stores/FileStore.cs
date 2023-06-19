using System.Text;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class FileStore
{
    private ApplicationDbContext _context;
    private string _letters;
    private Random _random;

    public FileStore(ApplicationDbContext context)
    {
        _context = context;
        _letters = Enumerable
            .Range('A', 26)
            .Concat(Enumerable.Range('a', 26))
            .Select(x => (char)x)
            .ToString()!;
        _random = new Random();
    }

    public async Task<List<Models.File>> InsertTestFiles(
        int numNonAssociatedFiles
    )
    {
        List<Models.File> files = new List<Models.File>();

        for (int _ = 0; _ < numNonAssociatedFiles; _++)
        {
            Models.File f = new Models.File
            {
                Name = GenerateTestFileName(10),
                StoreKey = GenerateTestFileKey(10),
            };
            files.Add(f);
        }

        await InsertFiles(files);

        return files;
    }

    public async Task<int> InsertFiles(List<Models.File> files)
    {
        _context.AddRange(files);
        return await _context.SaveChangesAsync();
    }

    private string GenerateTestFileName(int randsize) =>
        "test_name" + GenerateRandomString(randsize);

    private string GenerateTestFileKey(int randsize) =>
        "test_key" + GenerateRandomString(randsize);

    private string GenerateRandomString(int length)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int index = _random.Next(_letters.Length);
            builder.Append(_letters[index]);
        }

        return builder.ToString();
    }
}
