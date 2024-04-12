using Dapper;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Utils;
using PersistenceService.Utils.GraphQL;
using System.Linq.Dynamic.Core;
using GraphQLTypes = Common.SlackCloneGraphQL.Types;

namespace PersistenceService.Stores;

public class WorkspaceStore : Store
{
    private IEnumerable<string> WorkspaceProps { get; set; }

    public const string CHANNEL = "CHANNEL";

    public const string DIRECT_MESSAGE_GROUP = "DIRECT_MESSAGE_GROUP";

    public WorkspaceStore(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        WorkspaceProps = typeof(Workspace).GetProperties().Select(p => p.Name);
    }

    public async Task<List<Guid>> LoadUserGroups(Guid userId, Guid workspaceId)
    {
        var channelIds = await _context.ChannelMembers
            .Where(cm => cm.WorkspaceId == workspaceId)
            .Where(cm => cm.UserId == userId)
            .Select(cm => cm.ChannelId)
            .ToListAsync();
        var dmGroupIds = await _context.DirectMessageGroupMembers
            .Where(dm => dm.WorkspaceId == workspaceId)
            .Where(dm => dm.UserId == userId)
            .Select(dm => dm.DirectMessageGroupId)
            .ToListAsync();

        return channelIds.Concat(dmGroupIds).ToList();
    }

    public async Task<Workspace> CreateWorkspace(
        Workspace workspaceSkeleton,
        Guid creatorId,
        List<string>? invitedEmails
    )
    {
        Workspace workspace = (
            await InsertWorkspaces(
                new List<Workspace> { workspaceSkeleton },
                performSave: false
            )
        ).First();

        CreateWorkspaceAdmin(creatorId, workspace);
        if (invitedEmails is not null)
        {
            await InviteUsersByEmail(workspace, creatorId, invitedEmails);
        }
        await _context.SaveChangesAsync();

        return workspace;
    }

    private async Task<List<WorkspaceInvite>> InviteUsersByEmail(
        Workspace workspace,
        Guid adminId,
        List<string> emails
    )
    {
        List<Guid> idsFromEmails = await _context.Users
            .Where(u => emails.Contains(u.Email))
            .Select(u => u.Id)
            .ToListAsync();
        bool validEmails = idsFromEmails.Count == emails.Count;
        if (!validEmails)
        {
            throw new InvalidOperationException(
                "Could not invite to workspace"
            );
        }

        List<WorkspaceInvite> invites = new();
        foreach (Guid invitedId in idsFromEmails)
        {
            invites.Add(
                new WorkspaceInvite
                {
                    AdminId = adminId,
                    UserId = invitedId,
                    Workspace = workspace
                }
            );
        }

        _context.AddRange(invites);
        return invites;
    }

    private WorkspaceMember CreateWorkspaceAdmin(
        Guid userId,
        Workspace workspace,
        int permissionsMask = 1
    )
    {
        WorkspaceMember workspaceMembership = new WorkspaceMember
        {
            Admin = true,
            Title = "Admin",
            UserId = userId,
            Workspace = workspace,
        };
        _context.Add(workspaceMembership);
        workspace.NumMembers += 1;

        WorkspaceAdminPermissions permissions = new WorkspaceAdminPermissions
        {
            AdminId = userId,
            Workspace = workspace,
            WorkspaceAdminPermissionsMask = permissionsMask
        };
        _context.Add(permissions);
        workspaceMembership.WorkspaceAdminPermissions = permissions;

        return workspaceMembership;
    }

    public async Task<WorkspaceMember> InsertWorkspaceAdmin(
        Guid userId,
        Guid workspaceId,
        int permissionsMask = 1
    )
    {
        bool userExists =
            _context.Users.Where(u => u.Id == userId).Count() == 1;
        Workspace? workspace = _context.Workspaces
            .Where(w => w.Id == workspaceId)
            .FirstOrDefault();
        bool validMask = 0 <= permissionsMask && permissionsMask <= 2047;
        if (!userExists || workspace is null || !validMask)
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
            workspace.NumMembers += 1;
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
        Workspace? workspace = _context.Workspaces
            .Where(w => w.Id == workspaceId)
            .FirstOrDefault();
        if (workspace is null)
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

        workspace.NumMembers += workspaceMembers.Count;
        _context.AddRange(workspaceMembers);
        await _context.SaveChangesAsync();

        return workspaceMembers;
    }

    public async Task<List<Workspace>> InsertWorkspaces(
        List<Workspace> workspaces,
        bool performSave = true
    )
    {
        _context.AddRange(workspaces);
        if (performSave)
        {
            await _context.SaveChangesAsync();
        }
        return workspaces;
    }

    public async Task<List<WorkspaceInvite>> InviteUsersByEmail(
        Guid workspaceId,
        Guid adminId,
        List<string> emails
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

        List<Guid> idsFromEmails = await _context.Users
            .Where(u => emails.Contains(u.Email))
            .Select(u => u.Id)
            .ToListAsync();
        bool validEmails = idsFromEmails.Count == emails.Count;
        if (!validEmails)
        {
            throw new InvalidOperationException(
                "Could not invite to workspace"
            );
        }

        List<WorkspaceInvite> invites = new();
        foreach (Guid invitedId in idsFromEmails)
        {
            invites.Add(
                new WorkspaceInvite
                {
                    AdminId = adminId,
                    UserId = invitedId,
                    WorkspaceId = workspaceId
                }
            );
        }

        _context.AddRange(invites);
        await _context.SaveChangesAsync();
        return invites;
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
            _context.WorkspaceMembers
                .Where(
                    wm => wm.UserId == userId && wm.WorkspaceId == workspaceId
                )
                .Count() == 1;
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

    public async Task<(
        List<GraphQLTypes.Workspace> workspaces,
        bool lastPage
    )> LoadWorkspaces(
        Guid userId,
        int first,
        IEnumerable<string> cols,
        Guid? after = null
    )
    {
        string wWorkspaces = Stores.Store.wdq("Workspaces");
        string wWorkspaceMembers = Stores.Store.wdq("WorkspaceMembers");
        string wUserId = Stores.Store.wdq("UserId");
        string wId = Stores.Store.wdq("Id");
        string wWorkspaceId = Stores.Store.wdq("WorkspaceId");
        string wName = Stores.Store.wdq("Name");

        var sqlBuilder = new List<string>();

        sqlBuilder.Add("WITH workspaceMembers_ AS (");
        sqlBuilder.Add("SELECT");
        sqlBuilder.Add(wWorkspaceId);
        sqlBuilder.Add("FROM");
        sqlBuilder.Add($"{wWorkspaceMembers}");
        sqlBuilder.Add("WHERE");
        sqlBuilder.Add($"{wUserId} = @UserId");
        sqlBuilder.Add(after is null ? ")\n" : "),");

        if (after is not null)
        {
            sqlBuilder.Add("after_ AS (");
            sqlBuilder.Add("SELECT");
            sqlBuilder.Add(wName);
            sqlBuilder.Add("FROM");
            sqlBuilder.Add(wWorkspaces);
            sqlBuilder.Add("WHERE");
            sqlBuilder.Add($"{wId} = @AfterId");
            sqlBuilder.Add(")\n");
        }

        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            cols.Select(c => Stores.Store.wdq(c))
                .Select(
                    (c, i) =>
                        $"{wWorkspaces}.{(i == cols.Count() - 1 ? $"{c}" : $"{c},")}"
                )
        );
        sqlBuilder.Add($"FROM {wWorkspaces}\n");
        sqlBuilder.Add(
            $"INNER JOIN workspaceMembers_ ON {wWorkspaces}.{wId} = workspaceMembers_.{wWorkspaceId}"
        );
        if (after is not null)
        {
            sqlBuilder.Add("AND");
            sqlBuilder.Add($"{wName} > (SELECT {wName} FROM after_)");
        }
        sqlBuilder.Add($"ORDER BY {wName}");
        sqlBuilder.Add($"LIMIT @First;");

        var sql = string.Join("\n", sqlBuilder);
        var conn = _context.GetConnection();
        var parameters = new
        {
            UserId = userId,
            AfterId = after,
            First = first + 1
        };
        var workspaces = await conn.QueryAsync<GraphQLTypes.Workspace>(
            sql,
            parameters
        );
        List<GraphQLTypes.Workspace> res = workspaces.ToList();
        var lastPage = res.Count <= first;
        if (!lastPage)
        {
            res.RemoveAt(res.Count - 1);
        }

        return (res, lastPage);
    }

    public async Task<(
        List<dynamic> dbMembers,
        bool lastPage
    )> LoadWorkspaceMembers(
        Guid userId,
        int first,
        FieldTree connectionTree,
        Guid workspaceId,
        Guid? after = null
    )
    {
        IQueryable<WorkspaceMember> memberships = _context.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == workspaceId)
            .Include(wm => wm.User);
        if (!(after is null))
        {
            string prevLast = await memberships
                .Where(wm => wm.Id == after)
                .Select(wm => wm.User.NormalizedUserName)
                .FirstAsync();
            memberships = memberships.Where(
                wm => wm.User.NormalizedUserName.CompareTo(prevLast) > 0
            );
        }
        memberships = memberships
            .OrderBy(wm => wm.User.NormalizedUserName)
            .Take(first + 1);
        var dynamicWorkspaceMembers = await memberships
            .Select(
                DynamicLinqUtils.NodeFieldToDynamicSelectString(
                    connectionTree,
                    nonDbMapped: new List<string> { "workspaceMemberInfo" }
                )
            )
            .ToDynamicListAsync();

        bool lastPage = dynamicWorkspaceMembers.Count <= first;
        if (!lastPage)
        {
            dynamicWorkspaceMembers.RemoveAt(dynamicWorkspaceMembers.Count - 1);
        }
        return (dynamicWorkspaceMembers, lastPage);
    }

    public async Task<string> SignInUser(Guid userId, Guid workspaceId)
    {
        var member = await _context.WorkspaceMembers
            .Where(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId)
            .FirstAsync();
        member.OnlineStatus = "online";
        await _context.SaveChangesAsync();
        return "online";
    }

    public async Task<Workspace> GetWorkspace(Guid workspaceId)
    {
        return await _context.Workspaces
            .Where(w => w.Id == workspaceId)
            .FirstAsync();
    }

    public struct StarredInfo
    {
        public string Type { get; set; }
        public dynamic Starred { get; set; }
    }

    public async Task<(List<StarredInfo>, bool lastPage)> LoadStarred(
        Guid workspaceId,
        Guid userId,
        int first,
        FieldTree connectionTree,
        Guid? after = null
    )
    {
        IQueryable<Star> stars = _context.Stars
            .Where(s => s.UserId == userId && s.WorkspaceId == workspaceId)
            .OrderByDescending(s => s.CreatedAt);
        if (!(after is null))
        {
            DateTime afterStarredAt = stars
                .Where(
                    s => s.ChannelId == after || s.DirectMessageGroupId == after
                )
                .Select(s => s.CreatedAt)
                .First();
            stars = stars.Where(
                s =>
                    s.CreatedAt <= afterStarredAt
                    && s.ChannelId != after
                    && s.DirectMessageGroupId != after
            );
        }
        var starredRows = await stars
            .Take(first + 1)
            .Include(s => s.DirectMessageGroup)
            .ThenInclude(dmg => dmg.DirectMessageGroupMembers)
            .Select(
                s =>
                    new
                    {
                        s.Id,
                        s.CreatedAt,
                        s.Channel,
                        s.DirectMessageGroup
                    }
            )
            .ToListAsync();

        List<StarredInfo> starred = new();
        foreach (var row in starredRows)
        {
            if (row.Channel is not null)
            {
                starred.Add(
                    new StarredInfo { Type = CHANNEL, Starred = row.Channel }
                );
            }
            else
            {
                starred.Add(
                    new StarredInfo
                    {
                        Type = DIRECT_MESSAGE_GROUP,
                        Starred = row.DirectMessageGroup
                    }
                );
            }
        }

        bool lastPage = starred.Count <= first;
        if (!lastPage)
        {
            starred.RemoveAt(starred.Count - 1);
        }
        return (starred, lastPage);
    }
}
