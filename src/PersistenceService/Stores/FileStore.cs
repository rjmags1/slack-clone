using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class FileStore : Store
{
    public FileStore(ApplicationDbContext context)
        : base(context) { }

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

    public async Task<List<Models.File>> InsertFiles(List<Models.File> files)
    {
        _context.AddRange(files);
        await _context.SaveChangesAsync();
        return files;
    }

    private string GenerateTestFileName(int randsize) =>
        "test_file_name" + GenerateRandomString(randsize);

    private string GenerateTestFileKey(int randsize) =>
        "test_file_key" + GenerateRandomString(randsize);
}
