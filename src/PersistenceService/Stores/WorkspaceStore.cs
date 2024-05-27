using Dapper;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
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
        WorkspaceMember workspaceMembership =
            new()
            {
                Admin = true,
                Title = "Admin",
                UserId = userId,
                Workspace = workspace,
            };
        _context.Add(workspaceMembership);
        workspace.NumMembers += 1;

        WorkspaceAdminPermissions permissions =
            new()
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
        string wWorkspaces = wdq("Workspaces");
        string wWorkspaceMembers = wdq("WorkspaceMembers");
        string wUserId = wdq("UserId");
        string wId = wdq("Id");
        string wWorkspaceId = wdq("WorkspaceId");
        string wName = wdq("Name");
        string wAvatarId = wdq("AvatarId");
        string wFiles = wdq("Files");
        string wStoreKey = wdq("StoreKey");

        var sqlBuilder = new List<string>
        {
            "WITH workspaceMembers_ AS (",
            "SELECT",
            wWorkspaceId,
            "FROM",
            $"{wWorkspaceMembers}",
            "WHERE",
            $"{wUserId} = @UserId",
            after is null ? ")\n" : "),"
        };

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
                        $"{wWorkspaces}.{(i == cols.Count() - 1 && !cols.Contains("AvatarId") ? $"{c}" : $"{c},")}"
                )
        );
        if (cols.Contains("AvatarId"))
        {
            sqlBuilder.Add(
                $"{wFiles}.{wId}, {wFiles}.{wName}, {wFiles}.{wStoreKey}"
            );
        }

        sqlBuilder.Add($"FROM {wWorkspaces}\n");
        sqlBuilder.Add(
            $"INNER JOIN workspaceMembers_ ON {wWorkspaces}.{wId} = workspaceMembers_.{wWorkspaceId}"
        );
        if (cols.Contains("AvatarId"))
        {
            sqlBuilder.Add(
                $"LEFT JOIN {wFiles} ON {wFiles}.{wId} = {wWorkspaces}.{wAvatarId}"
            );
        }
        if (after is not null)
        {
            sqlBuilder.Add("WHERE");
            sqlBuilder.Add(
                $"{wWorkspaces}.{wName} > (SELECT {wName} FROM after_)"
            );
        }
        sqlBuilder.Add($"ORDER BY {wWorkspaces}.{wName}");
        sqlBuilder.Add($"LIMIT @First;");

        var sql = string.Join("\n", sqlBuilder);
        var conn = _context.GetConnection();
        var parameters = new
        {
            UserId = userId,
            AfterId = after,
            First = first + 1
        };
        var workspaces = await conn.QueryAsync<
            GraphQLTypes.Workspace,
            GraphQLTypes.File,
            GraphQLTypes.Workspace
        >(
            sql: sql,
            param: parameters,
            map: (workspace, avatar) =>
            {
                workspace.Avatar = avatar;
                return workspace;
            }
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
        List<GraphQLTypes.WorkspaceMember> dbMembers,
        bool lastPage
    )> LoadWorkspaceMembers(
        int first,
        List<string> dbCols,
        Guid workspaceId,
        Guid? after = null
    )
    {
        var throwOn = new string[]
        {
            "WorkspaceAdminPermissionsId",
            "NotificationsAllowTimeStart",
            "NotificationsAllowTimeEnd",
            "NotificationSound",
            "ThemeId"
        };
        if (dbCols.Any(c => throwOn.Contains(c)))
        {
            throw new InvalidOperationException(
                "attempted to load private member information"
            );
        }
        dbCols.Remove("Id");
        dbCols.Insert(0, "Id");
        dbCols.Remove("AvatarId");
        dbCols.Add("AvatarId");

        List<string> sqlBuilder = new();
        if (after is not null)
        {
            sqlBuilder.Add("WITH after AS (");
            sqlBuilder.Add(
                $"SELECT {wdq("UserId")} FROM {wdq("WorkspaceMembers")}"
            );
            sqlBuilder.Add($"WHERE {wdq("Id")} = @AfterId");
            sqlBuilder.Add("),\n");

            sqlBuilder.Add("afterName AS (");
            sqlBuilder.Add(
                $"SELECT {wdq("UserName")} FROM {wdq("AspNetUsers")}"
            );
            sqlBuilder.Add(
                $"WHERE {wdq("AspNetUsers")}.{wdq("Id")} = (SELECT {wdq("UserId")} FROM after)"
            );
            sqlBuilder.Add("),\n");
        }
        else
        {
            sqlBuilder.Add("WITH");
        }

        sqlBuilder.Add("members AS (");
        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            dbCols.Select(
                (c, i) => i == dbCols.Count - 1 ? $"{wdq(c)}" : $"{wdq(c)},"
            )
        );
        sqlBuilder.Add(
            $"FROM {wdq("WorkspaceMembers")} WHERE {wdq("WorkspaceId")} = @WorkspaceId"
        );
        sqlBuilder.Add(")\n");

        sqlBuilder.Add(
            $"SELECT members.*, {wdq("AspNetUsers")}.{wdq("Id")}, {wdq("AspNetUsers")}.{wdq("UserName")} AS {wdq("Username")},"
        );
        sqlBuilder.Add(
            $"{wdq("Files")}.{wdq("Id")}, {wdq("Files")}.{wdq("StoreKey")}, {wdq("Files")}.{wdq("Name")}"
        );
        sqlBuilder.Add($"FROM members INNER JOIN {wdq("AspNetUsers")} ON");
        sqlBuilder.Add(
            $"{wdq("AspNetUsers")}.{wdq("Id")} = members.{wdq("UserId")}"
        );
        sqlBuilder.Add(
            $"LEFT JOIN {wdq("Files")} ON {wdq("Files")}.{wdq("Id")} = members.{wdq("AvatarId")}"
        );
        if (after is not null)
        {
            sqlBuilder.Add(
                $"WHERE {wdq("AspNetUsers")}.{wdq("UserName")} > (SELECT {wdq("UserName")} FROM afterName)"
            );
        }
        sqlBuilder.Add($"ORDER BY {wdq("AspNetUsers")}.{wdq("UserName")}");
        sqlBuilder.Add("LIMIT @First");

        var sql = string.Join("\n", sqlBuilder);
        var param = new
        {
            AfterId = after,
            WorkspaceId = workspaceId,
            First = first + 1
        };
        var conn = _context.GetConnection();

        List<GraphQLTypes.WorkspaceMember> members = (
            await conn.QueryAsync<
                Models.WorkspaceMember,
                GraphQLTypes.User,
                GraphQLTypes.File,
                GraphQLTypes.WorkspaceMember
            >(
                sql: sql,
                param: param,
                map: (modelMember, user, avatar) =>
                {
                    var member = new GraphQLTypes.WorkspaceMember
                    {
                        Id = modelMember.Id,
                        Avatar = avatar,
                        JoinedAt = modelMember.JoinedAt,
                        Title = modelMember.Title,
                        User = user,
                        Workspace = new GraphQLTypes.Workspace
                        {
                            Id = modelMember.WorkspaceId
                        },
                        WorkspaceMemberInfo =
                            new GraphQLTypes.WorkspaceMemberInfo
                            {
                                Admin = modelMember.Admin,
                                Owner = modelMember.Owner,
                                OnlineStatus = modelMember.OnlineStatus,
                                OnlineStatusUntil =
                                    modelMember.OnlineStatusUntil
                            }
                    };

                    return member;
                }
            )
        ).ToList();

        var lastPage = members.Count <= first;
        if (!lastPage)
        {
            members.RemoveAt(members.Count - 1);
        }

        return (members, lastPage);
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

    public async Task<GraphQLTypes.Workspace> GetWorkspace(
        Guid workspaceId,
        List<string> dbCols
    )
    {
        if (dbCols.Contains("members"))
        {
            throw new NotImplementedException();
        }

        var wWorkspaces = wdq("Workspaces");
        var wId = wdq("Id");
        var wAvatarId = wdq("AvatarId");
        var wFiles = wdq("Files");
        var wName = wdq("Name");
        var wStoreKey = wdq("StoreKey");

        List<string> sqlBuilder = new();
        sqlBuilder.Add("WITH workspace AS (");
        sqlBuilder.Add($"SELECT * FROM {wWorkspaces}");
        sqlBuilder.Add($"WHERE {wWorkspaces}.{wId} = @WorkspaceId");
        sqlBuilder.Add(")");
        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            dbCols.Select(
                (c, i) =>
                    $"workspace.{(i < dbCols.Count - 1 ? wdq(c) + "," : wdq(c))}"
            )
        );
        if (dbCols.Contains("AvatarId"))
        {
            sqlBuilder.Add(",");
            sqlBuilder.Add(
                $"{wFiles}.{wId}, {wFiles}.{wName}, {wFiles}.{wStoreKey}"
            );
        }
        sqlBuilder.Add("FROM workspace");
        if (dbCols.Contains("AvatarId"))
        {
            sqlBuilder.Add(
                $"LEFT JOIN {wFiles} ON {wFiles}.{wId} = workspace.{wAvatarId}"
            );
        }
        sqlBuilder.Add(";");

        var sql = string.Join("\n", sqlBuilder);
        var param = new { WorkspaceId = workspaceId };
        var conn = _context.GetConnection();
        var workspace = (
            !dbCols.Contains("AvatarId")
                ? await conn.QueryAsync<GraphQLTypes.Workspace>(sql, param)
                : await conn.QueryAsync<
                    GraphQLTypes.Workspace,
                    GraphQLTypes.File,
                    GraphQLTypes.Workspace
                >(
                    sql: sql,
                    param: param,
                    map: (workspace, avatar) =>
                    {
                        workspace.Avatar = avatar;
                        return workspace;
                    }
                )
        ).First();

        return workspace;
    }

    public async Task<(List<GraphQLTypes.Group>, bool lastPage)> LoadStarred(
        Guid workspaceId,
        Guid userId,
        int first,
        List<string> dbCols,
        Guid? after = null
    )
    {
        if (dbCols.Contains("Name"))
        {
            dbCols.Remove("Name");
            dbCols.Add("Name");
        }

        List<string> sqlBuilder = new();
        if (after is not null)
        {
            sqlBuilder.Add("WITH after AS (");
            sqlBuilder.Add($"SELECT {wdq("CreatedAt")}");
            sqlBuilder.Add(
                @$"FROM {wdq("Stars")} WHERE {wdq("DirectMessageGroupId")} = @AfterId 
                    OR {wdq("ChannelId")} = @AfterId"
            );
            sqlBuilder.Add("),\n");
        }
        else
        {
            sqlBuilder.Add("WITH");
        }

        sqlBuilder.Add("stars AS (");
        sqlBuilder.Add(
            $"SELECT {wdq("Id")}, {wdq("CreatedAt")}, {wdq("ChannelId")}, {wdq("DirectMessageGroupId")}"
        );
        sqlBuilder.Add($"FROM {wdq("Stars")}");
        sqlBuilder.Add(
            $"WHERE {wdq("UserId")} = @UserId AND {wdq("WorkspaceId")} = @WorkspaceId"
        );
        if (after is not null)
        {
            sqlBuilder.Add(
                $"AND {wdq("CreatedAt")} > (SELECT {wdq("CreatedAt")} FROM after)"
            );
        }
        sqlBuilder.Add("LIMIT @First");
        sqlBuilder.Add("),\n");

        sqlBuilder.Add("starred_channels AS (");
        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            dbCols.Select(c => $"{wdq("Channels")}.{wdq(c)}, ")
        );
        sqlBuilder.Add($"stars.{wdq("CreatedAt")} AS {wdq("StarredAt")},");
        sqlBuilder.Add($"'Channel' AS {wdq("Type")},");
        sqlBuilder.Add($"NULL AS {wdq("UserName")}");
        sqlBuilder.Add($"FROM stars INNER JOIN {wdq("Channels")}");
        sqlBuilder.Add(
            $"ON stars.{wdq("ChannelId")} = {wdq("Channels")}.{wdq("Id")}"
        );
        sqlBuilder.Add("),\n");

        sqlBuilder.Add("starred_dmgs AS (");
        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            dbCols
                .Where(c => c != "Name")
                .Select(c => $"{wdq("DirectMessageGroups")}.{wdq(c)}, ")
        );
        if (dbCols.Contains("Name"))
        {
            sqlBuilder.Add($"NULL AS {wdq("Name")},");
        }
        sqlBuilder.Add($"stars.{wdq("CreatedAt")} AS {wdq("StarredAt")},");
        sqlBuilder.Add($"'DirectMessageGroup' AS {wdq("Type")},");
        sqlBuilder.Add($"{wdq("AspNetUsers")}.{wdq("UserName")}");
        sqlBuilder.Add($"FROM stars INNER JOIN {wdq("DirectMessageGroups")}");
        sqlBuilder.Add(
            $"ON stars.{wdq("DirectMessageGroupId")} = {wdq("DirectMessageGroups")}.{wdq("Id")}"
        );
        sqlBuilder.Add(
            $"INNER JOIN {wdq("DirectMessageGroupMembers")} ON {wdq("DirectMessageGroupMembers")}.{wdq("DirectMessageGroupId")} = {wdq("DirectMessageGroups")}.{wdq("Id")}"
        );
        sqlBuilder.Add(
            $"INNER JOIN {wdq("AspNetUsers")} ON {wdq("AspNetUsers")}.{wdq("Id")} = {wdq("DirectMessageGroupMembers")}.{wdq("UserId")}"
        );
        sqlBuilder.Add(")\n");

        sqlBuilder.Add(
            "SELECT * FROM starred_channels UNION SELECT * FROM starred_dmgs;"
        );

        var sql = string.Join("\n", sqlBuilder);

        var param = new
        {
            WorkspaceId = workspaceId,
            AfterId = after,
            UserId = userId,
            First = first + 1
        };
        var conn = _context.GetConnection();
        List<GraphQLTypes.Group> groups = (
            await conn.QueryAsync<
                GraphQLTypes.Group,
                Models.User,
                GraphQLTypes.Group
            >(
                sql,
                param: param,
                map: (group, user) =>
                {
                    if (group.Type == "DirectMessageGroup")
                    {
                        group.Name = user.UserName;
                    }
                    return group;
                },
                splitOn: "UserName"
            )
        ).ToList();

        var channelGroups = groups.Where(g => g.Type == "Channel");
        var dmgGroups = groups
            .Where(g => g.Type == "DirectMessageGroup")
            .GroupBy(g => g.Id)
            .Select(g =>
            {
                var dmg = g.First();
                var names = g.Select(g => g.Name).ToList();
                dmg.Name = string.Join(", ", names);
                return dmg;
            });
        groups = channelGroups
            .Concat(dmgGroups)
            .OrderByDescending(g => g.StarredAt)
            .ToList();
        var lastPage = groups.Count <= first;
        if (!lastPage)
        {
            groups.RemoveAt(groups.Count - 1);
        }

        return (groups, lastPage);
    }
}
