using PersistenceService.Data.ApplicationDb;

namespace PersistenceService.Stores;

public class FileStore : Store
{
    public FileStore(ApplicationDbContext context)
        : base(context) { }

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
