using Dapper;
using PersistenceService.Data.ApplicationDb;
using GraphQLTypes = Common.SlackCloneGraphQL.Types;

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

    public async Task<GraphQLTypes.File?> GetFileById(Guid? id)
    {
        var sql = $"SELECT * FROM {wdq("Files")} WHERE {wdq("Id")} = @FileId";
        var param = new { FileId = id };
        var conn = _context.GetConnection();

        var file = (
            await conn.QueryAsync<GraphQLTypes.File>(sql, param)
        ).FirstOrDefault();

        return file;
    }

    private string GenerateTestFileName(int randsize) =>
        "test_file_name" + GenerateRandomString(randsize);

    private string GenerateTestFileKey(int randsize) =>
        "test_file_key" + GenerateRandomString(randsize);
}
