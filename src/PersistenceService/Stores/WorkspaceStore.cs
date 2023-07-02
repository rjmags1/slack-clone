using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace PersistenceService.Stores;

public class WorkspaceStore : Store
{
    public WorkspaceStore(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<List<WorkspaceMember>> InsertWorkspaceMembers(
        Guid workspaceId,
        List<Guid> userIds,
        List<string> titles
    )
    {
        bool workspaceExists =
            _context.Workspaces.Where(w => w.Id == workspaceId).Count() == 1;
        if (!workspaceExists)
        {
            throw new InvalidOperationException("Could not invite users");
        }
        bool allUsersExist =
            _context.Users.Where(u => userIds.Contains(u.Id)).Count()
            == userIds.Count;
        if (!allUsersExist)
        {
            throw new InvalidOperationException("Could not invite users");
        }
        bool allUsersNotMembers =
            _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId)
                .Where(wm => userIds.Contains(wm.UserId))
                .Count() == 0;
        if (!allUsersNotMembers)
        {
            throw new InvalidOperationException("Could not invite users");
        }

        List<WorkspaceMember> workspaceMembers = new List<WorkspaceMember>();
        foreach ((Guid userId, string title) in userIds.Zip(titles))
        {
            workspaceMembers.Add(
                new WorkspaceMember
                {
                    Title = title,
                    WorkspaceId = workspaceId,
                    UserId = userId
                }
            );
        }

        _context.AddRange(workspaceMembers);
        await _context.SaveChangesAsync();

        return workspaceMembers;
    }

    public async Task<List<Workspace>> InsertWorkspaces(
        List<Workspace> workspaces
    )
    {
        _context.AddRange(workspaces);
        await _context.SaveChangesAsync();
        return workspaces;
    }

    public async Task<WorkspaceInvite> InsertWorkspaceInvite(
        Guid workspaceId,
        Guid adminId,
        Guid userId
    )
    {
        bool workspaceExists =
            _context.Workspaces.Where(w => w.Id == workspaceId).Count() == 1;
        if (!workspaceExists)
        {
            throw new InvalidOperationException(
                "Could not invite to workspace"
            );
        }

        WorkspaceMember adminMembership = _context.WorkspaceMembers.First(
            wm => wm.UserId == adminId && wm.WorkspaceId == workspaceId
        );
        if (!adminMembership.Admin)
        {
            throw new InvalidOperationException(
                "Only workspace admins may send invites"
            );
        }

        bool invitedExists =
            _context.Users.Where(u => u.Id == userId).Count() == 1;
        bool invitedAlreadyWorkspaceMember =
            _context.WorkspaceMembers.Where(wm => wm.UserId == userId).Count()
            == 1;
        if (!invitedExists || invitedAlreadyWorkspaceMember)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        WorkspaceInvite invite = new WorkspaceInvite
        {
            AdminId = adminId,
            UserId = userId,
            WorkspaceId = workspaceId
        };
        _context.Add(invite);
        await _context.SaveChangesAsync();

        return invite;
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
