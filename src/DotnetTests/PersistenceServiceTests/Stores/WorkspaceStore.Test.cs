using DotnetTests.Fixtures;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTest.PersistenceService.Stores;

[Collection("Database collection")]
public class WorkspaceStoreTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WorkspaceStore _workspaceStore;

    public WorkspaceStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _workspaceStore = new WorkspaceStore(_dbContext);
    }

    [Fact]
    public async void InsertWorkspaces_ShouldInsertWorkspaces()
    {
        List<Workspace> workspaces = new List<Workspace>();
        for (int i = 0; i < 10; i++)
        {
            workspaces.Add(
                new Workspace
                {
                    Description = "test description" + i.ToString(),
                    Name = "test-workspace-name" + i.ToString()
                }
            );
        }

        List<Workspace> inserted = await _workspaceStore.InsertWorkspaces(
            workspaces
        );
        foreach ((Workspace iw, Workspace w) in inserted.Zip(workspaces))
        {
            Assert.NotEqual(iw.Id, Guid.Empty);
            Assert.Null(iw.Avatar);
            Assert.Null(iw.AvatarId);
            Assert.NotEqual(iw.ConcurrencyStamp, Guid.Empty);
            Assert.NotEqual(iw.CreatedAt, default(DateTime));
            Assert.Equal(iw.Description, w.Description);
            Assert.Equal(iw.Name, w.Name);
            Assert.Equal(1, iw.NumMembers);
        }
    }

    [Fact]
    public async void InsertTestWorkspaces_ShouldInsertTestWorkspaces()
    {
        int initialWorkspaces = _dbContext.Workspaces.Count();
        await _workspaceStore.InsertTestWorkspaces(100);
        int currentWorkspaces = _dbContext.Workspaces.Count();
        Assert.Equal(initialWorkspaces + 100, currentWorkspaces);
    }
}
