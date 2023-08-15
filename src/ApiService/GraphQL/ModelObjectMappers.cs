using System.Text.Json;
using ApiService.Utils;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using File = SlackCloneGraphQL.Types.File;
using Models = PersistenceService.Models;

namespace SlackCloneGraphQL;

/// <summary>
/// This class performs data transfer utility functions that enable the
/// translation of both EF Core Model, and anonymous objects resulting
/// from optimized dynamic LINQ
/// queries, into objects that can be understood by GraphQL.NET and translated
/// into GraphQL responses.
///
/// The methods of the class that handle anonymous object LINQ query results convert
/// them into duck-typed System.Dynamic.Expando objects that implement
/// IDictionary. The conversion is done via json serialization
/// and is useful because it provides a way to check for the presence of individual
/// members of EF Core model objects (which the expando objects originate from).
/// </summary>
public static class ModelToObjectConverters
{
    private const string DEFAULT_ONLINE_STATUS = "offline";

    private static readonly File DefaultAvatar = new File
    {
        Id = Guid.Empty,
        Name = "DEFAULT_AVATAR",
        StoreKey = "DEFAULT_AVATAR",
        UploadedAt = default(DateTime)
    };

    private static readonly Theme DefaultTheme = new Theme
    {
        Id = Guid.Empty,
        Name = "DEFAULT_THEME"
    };

    public static Workspace ConvertDynamicWorkspace(dynamic modelWorkspace)
    {
        var expando = DynamicUtils.ToExpando(modelWorkspace);
        Workspace workspace = new Workspace();
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Id)))
        {
            workspace.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Avatar)))
        {
            if (expando.Avatar is null)
            {
                workspace.Avatar = DefaultAvatar;
            }
            else
            {
                Models.File dbAvatar = JsonSerializer.Deserialize<Models.File>(
                    expando.Avatar
                );
                workspace.Avatar = ConvertAvatar(dbAvatar);
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.CreatedAt)))
        {
            workspace.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Description)))
        {
            workspace.Description = JsonSerializer.Deserialize<string>(
                expando.Description
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Name)))
        {
            workspace.Name = JsonSerializer.Deserialize<string>(expando.Name);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.NumMembers)))
        {
            workspace.NumMembers = JsonSerializer.Deserialize<int>(
                expando.NumMembers
            );
        }

        return workspace;
    }

    public static User ConvertUser(
        Models.User modelUser,
        IEnumerable<string> requestedFields
    )
    {
        User user = new User
        {
            Id = modelUser.Id,
            Avatar = ConvertAvatar(modelUser.Avatar),
            OnlineStatus = modelUser.OnlineStatus ?? DEFAULT_ONLINE_STATUS,
            OnlineStatusUntil = modelUser.OnlineStatusUntil,
            Username = modelUser.UserName,
            CreatedAt = modelUser.CreatedAt,
        };
        if (
            requestedFields.Contains(
                StringUtils.ToLowerFirstLetter(nameof(User.PersonalInfo))
            )
        )
        {
            UserInfo userInfo = new UserInfo
            {
                Email = modelUser.Email,
                EmailConfirmed = modelUser.EmailConfirmed,
                FirstName = modelUser.FirstName,
                LastName = modelUser.LastName,
                Theme = ConvertTheme(modelUser.Theme),
                Timezone = modelUser.Timezone,
                UserNotificationsPreferences =
                    ConvertUserNotificationsPreferences(modelUser)
            };
            user.PersonalInfo = userInfo;
        }

        return user;
    }

    public static File ConvertAvatar(Models.File? avatar)
    {
        return avatar is null
            ? DefaultAvatar
            : new File
            {
                Id = avatar.Id,
                Name = avatar.Name,
                StoreKey = avatar.StoreKey,
                UploadedAt = avatar.UploadedAt
            };
    }

    public static Theme ConvertTheme(Models.Theme? theme)
    {
        return theme is null
            ? DefaultTheme
            : new Theme { Id = theme.Id, Name = theme.Name };
    }

    public static UserNotificationsPreferences ConvertUserNotificationsPreferences(
        Models.User modelUser
    )
    {
        int mask = modelUser.UserNotificationsPreferencesMask;
        return new UserNotificationsPreferences
        {
            AllMessages = (mask & 1) > 0,
            NoMessages = (mask & 2) > 0,
            Mentions = (mask & 4) > 0,
            DMs = (mask & 8) > 0,
            Replies = (mask & 16) > 0,
            ThreadWatch = (mask & 32) > 0,
            NotifSound = modelUser.NotificationSound,
            AllowAlertsStartTimeUTC = modelUser.NotificationsAllowStartTime,
            AllowAlertsEndTimeUTC = modelUser.NotificationsAllowEndTime,
            PauseAlertsUntil = modelUser.NotificationsPauseUntil
        };
    }

    public static Connection<T> ToConnection<T>(
        List<T> nodes,
        bool firstPage,
        bool lastPage
    )
        where T : INode
    {
        return new Connection<T>
        {
            TotalEdges = nodes.Count,
            Edges = nodes
                .Select(n => new ConnectionEdge<T> { Node = n, Cursor = n.Id })
                .ToList(),
            PageInfo = new PageInfo
            {
                StartCursor = nodes.Count > 0 ? nodes.First().Id : null,
                EndCursor = nodes.Count > 0 ? nodes.Last().Id : null,
                HasPreviousPage = !firstPage,
                HasNextPage = !lastPage
            }
        };
    }

    public static WorkspaceMember ConvertDynamicWorkspaceMember(
        dynamic modelWorkspaceMember,
        List<string> userFields,
        List<string> adminFields
    )
    {
        var expando = DynamicUtils.ToExpando(modelWorkspaceMember);
        WorkspaceMember member = new WorkspaceMember();
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Id)))
        {
            member.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Avatar)))
        {
            if (expando.Avatar is null)
            {
                member.Avatar = DefaultAvatar;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.JoinedAt)))
        {
            member.JoinedAt = JsonSerializer.Deserialize<DateTime>(
                expando.JoinedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Title)))
        {
            member.Title = JsonSerializer.Deserialize<string>(expando.Title);
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.User)))
        {
            Models.User modelUser = JsonSerializer.Deserialize<Models.User>(
                expando.User
            );
            member.User = ConvertUser(modelUser, userFields);
        }
        if (
            DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Workspace))
        )
        {
            Models.Workspace modelWorkspace =
                JsonSerializer.Deserialize<Models.Workspace>(expando.Workspace);
            member.Workspace = ConvertWorkspace(modelWorkspace);
        }
        if (IncludeWorkspaceMemberInfo(expando))
        {
            member.WorkspaceMemberInfo = ConvertWorkspaceMemberInfo(
                expando,
                adminFields
            );
        }

        return member;
    }

    public static WorkspaceMemberInfo ConvertWorkspaceMemberInfo(
        dynamic expandoModelWorkspaceMember,
        List<string> adminFields
    )
    {
        WorkspaceMemberInfo memberInfo = new WorkspaceMemberInfo();
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.Admin)
            )
        )
        {
            memberInfo.Admin = JsonSerializer.Deserialize<bool>(
                expandoModelWorkspaceMember.Admin
            );
        }
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.Owner)
            )
        )
        {
            memberInfo.Owner = JsonSerializer.Deserialize<bool>(
                expandoModelWorkspaceMember.Owner
            );
        }
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.WorkspaceAdminPermissions)
            )
            && !(expandoModelWorkspaceMember.WorkspaceAdminPermissions is null)
        )
        {
            Models.WorkspaceAdminPermissions modelPermissions =
                JsonSerializer.Deserialize<Models.WorkspaceAdminPermissions>(
                    expandoModelWorkspaceMember.WorkspaceAdminPermissions
                );
            memberInfo.WorkspaceAdminPermissions =
                ConvertWorkspaceAdminPermissions(modelPermissions, adminFields);
        }

        return memberInfo;
    }

    public static WorkspaceAdminPermissions ConvertWorkspaceAdminPermissions(
        Models.WorkspaceAdminPermissions modelPermissions,
        List<string> adminFields
    )
    {
        int mask = modelPermissions.WorkspaceAdminPermissionsMask;
        return new WorkspaceAdminPermissions
        {
            Admin = ConvertUser(modelPermissions.Admin, adminFields),
            All = (mask & 1) > 0,
            Invite = (mask & 2) > 0,
            Kick = (mask & 4) > 0,
            AdminGrant = (mask & 8) > 0,
            AdminRevoke = (mask & 16) > 0,
            GrantAdminPermissions = (mask & 32) > 0,
            RevokeAdminPermissions = (mask & 64) > 0,
            EditMessages = (mask & 128) > 0,
            DeleteMessages = (mask & 256) > 0
        };
    }

    public static Workspace ConvertWorkspace(Models.Workspace modelWorkspace)
    {
        return new Workspace
        {
            Id = modelWorkspace.Id,
            Avatar = ConvertAvatar(modelWorkspace.Avatar),
            CreatedAt = modelWorkspace.CreatedAt,
            Description = modelWorkspace.Description,
            Name = modelWorkspace.Name,
            NumMembers = modelWorkspace.NumMembers
        };
    }

    private static bool IncludeWorkspaceMemberInfo(dynamic expando)
    {
        return (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Admin)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.WorkspaceAdminPermissions)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Owner)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationsAllowTimeStart)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationsAllTimeEnd)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationSound)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Theme)
            )
        );
    }
}
