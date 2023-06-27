using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace PersistenceService.Stores;

public class WorkspaceStore : Store
{
    public WorkspaceStore(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<List<Workspace>> InsertWorkspaces(
        List<Workspace> workspaces
    )
    {
        _context.AddRange(workspaces);
        await _context.SaveChangesAsync();
        return workspaces;
    }

    public async Task<List<Workspace>> InsertTestWorkspaces(
        int numTestWorkspaces
    )
    {
        List<Workspace> workspaces = new List<Workspace>();
        for (int i = 0; i < numTestWorkspaces; i++)
        {
            workspaces.Add(
                new Workspace
                {
                    Description = "test description",
                    Name = "test-workspace-name" + i.ToString()
                }
            );
        }

        _context.AddRange(workspaces);
        await _context.SaveChangesAsync();

        return workspaces;
    }
}
