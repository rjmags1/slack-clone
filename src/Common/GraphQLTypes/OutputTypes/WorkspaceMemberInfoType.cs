using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspaceMemberInfoType : ObjectGraphType<WorkspaceMemberInfo>
{
    public WorkspaceMemberInfoType()
    {
        Name = "WorkspaceMemberInfo";
        Field<NonNullGraphType<BooleanGraphType>>("admin")
            .Description("If the user is an admin of the workspace")
            .Resolve(context => context.Source.Admin);
        Field<NonNullGraphType<BooleanGraphType>>("owner")
            .Description("If the user is an owner of the workspace")
            .Resolve(context => context.Source.Owner);
        Field<WorkspaceAdminPermissionsType>("workspaceAdminPermissions")
            .Description(
                "Admin permissions that user has, if they have admin status"
            )
            .Resolve(context => context.Source.WorkspaceAdminPermissions);
        Field<NonNullGraphType<ThemeType>>("theme")
            .Description("Theme for this workspace")
            .Resolve(context => context.Source.Theme);
        Field<NonNullGraphType<IntGraphType>>("notifSound")
            .Description("Notification sound")
            .Resolve(context => context.Source.NotificationSound);
        Field<TimeOnlyGraphType>("notificationsAllowTimeStartUTC")
            .Description(
                "Start time during the day users start getting notifications from the workspace"
            )
            .Resolve(context => context.Source.NotificationsAllowTimeStart);
        Field<TimeOnlyGraphType>("notificationsAllowTimeEndUTC")
            .Description(
                "End time during the day users stop getting notifications from the workspace"
            )
            .Resolve(context => context.Source.NotificationsAllowTimeStart);
        Field<StringGraphType>("onlineStatus")
            .Description("Online status for this workspace")
            .Resolve(context => context.Source.OnlineStatus);
        Field<DateTimeGraphType>("onlineStatusUntilUTC")
            .Description(
                "Display the current online status until this timestamp"
            )
            .Resolve(context => context.Source.OnlineStatusUntil);
    }
}

public class WorkspaceMemberInfo
{
    public bool Admin { get; set; }
    public bool Owner { get; set; }
    public WorkspaceAdminPermissions? WorkspaceAdminPermissions { get; set; } =
        null!;
    public Theme Theme { get; set; } = null!;
    public int NotificationSound { get; set; }
    public TimeOnly? NotificationsAllowTimeStart { get; set; }
    public TimeOnly? NotificationsAllowTimeEnd { get; set; }
    public string? OnlineStatus { get; set; } = null!;
    public DateTime? OnlineStatusUntil { get; set; }
}
