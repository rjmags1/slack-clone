using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace PersistenceService.Stores;

public class WorkspaceStore : Store
{
    public WorkspaceStore(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<WorkspaceMember> InsertWorkspaceAdmin(
        Guid userId,
        Guid workspaceId,
        int permissionsMask = 1
    )
    {
        bool userExists =
            _context.Users.Where(u => u.Id == userId).Count() == 1;
        bool workspaceExists =
            _context.Workspaces.Where(w => w.Id == workspaceId).Count() == 1;
        bool validMask = 0 <= permissionsMask && permissionsMask <= 2047;
        if (!userExists || !workspaceExists || !validMask)
        {
            throw new InvalidOperationException(
                "Could not make the user an admin for the specified workspace"
            );
        }

        var workspaceMembership = _context.WorkspaceMembers
            .Where(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId)
            .FirstOrDefault();
        if (workspaceMembership is not null)
        {
            if (workspaceMembership.Admin)
            {
                throw new InvalidOperationException(
                    "Could not make the user admin"
                );
            }
            workspaceMembership.Admin = true;
        }
        else
        {
            workspaceMembership = new WorkspaceMember
            {
                Admin = true,
                Title = "Admin",
                UserId = userId,
                WorkspaceId = workspaceId,
            };
            _context.Add(workspaceMembership);
        }

        WorkspaceAdminPermissions permissions = new WorkspaceAdminPermissions
        {
            AdminId = userId,
            WorkspaceId = workspaceId,
            WorkspaceAdminPermissionsMask = permissionsMask
        };
        _context.Add(permissions);
        workspaceMembership.WorkspaceAdminPermissions = permissions;

        await _context.SaveChangesAsync();

        return workspaceMembership;
    }

    public async Task<WorkspaceSearch> InsertWorkspaceSearch(
        Guid workspaceId,
        Guid userId,
        string query
    )
    {
        bool validSearch =
            query.Length > 0
            && _context.WorkspaceMembers
                .Where(
                    wm => wm.UserId == userId && wm.WorkspaceId == workspaceId
                )
                .Count() == 1;
        if (!validSearch)
        {
            throw new InvalidOperationException(
                "Could not insert workspace search record"
            );
        }

        WorkspaceSearch search = new WorkspaceSearch
        {
            Query = query,
            UserId = userId,
            WorkspaceId = workspaceId
        };

        _context.Add(search);
        await _context.SaveChangesAsync();
        return search;
    }

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
