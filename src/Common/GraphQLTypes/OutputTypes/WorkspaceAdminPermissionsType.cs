using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspaceAdminPermissionsType
    : ObjectGraphType<WorkspaceAdminPermissions>
{
    public WorkspaceAdminPermissionsType(ISlackCloneData data)
    {
        Name = "WorkspaceAdminPermissions";
        Field<NonNullGraphType<UserType>>("admin")
            .Description("The user associated with the workspace permissions")
            .Resolve(context => context.Source.Admin);
        Field<NonNullGraphType<BooleanGraphType>>("all")
            .Description("Toggle on all permissions")
            .Resolve(context => context.Source.All);
        Field<NonNullGraphType<BooleanGraphType>>("invite")
            .Description("Permission to invite people to the workspace")
            .Resolve(context => context.Source.Invite);
        Field<NonNullGraphType<BooleanGraphType>>("kick")
            .Description("Permission to kick people from the workspace")
            .Resolve(context => context.Source.Kick);
        Field<NonNullGraphType<BooleanGraphType>>("adminGrant")
            .Description(
                "Permission to grant people admin status in the workspace"
            )
            .Resolve(context => context.Source.AdminGrant);
        Field<NonNullGraphType<BooleanGraphType>>("adminRevoke")
            .Description("Permission to revoke workspace admin status")
            .Resolve(context => context.Source.AdminRevoke);
        Field<NonNullGraphType<BooleanGraphType>>("grantAdminPermissions")
            .Description(
                "Permission to grant admins of the workspace specific permissions"
            )
            .Resolve(context => context.Source.GrantAdminPermissions);
        Field<NonNullGraphType<BooleanGraphType>>("revokeAdminPermissions")
            .Description(
                "Permission to revoke specific permissions from admins of the workspace"
            )
            .Resolve(context => context.Source.RevokeAdminPermissions);
        Field<NonNullGraphType<BooleanGraphType>>("editMessages")
            .Description("Permission to edit messages")
            .Resolve(context => context.Source.EditMessages);
        Field<NonNullGraphType<BooleanGraphType>>("deleteMessages")
            .Description("Permission to delete messages")
            .Resolve(context => context.Source.DeleteMessages);
    }
}

public class WorkspaceAdminPermissions
{
#pragma warning disable CS8618
    public User Admin { get; set; }
#pragma warning restore CS8618
    public bool All { get; set; }
    public bool Invite { get; set; }
    public bool Kick { get; set; }
    public bool AdminGrant { get; set; }
    public bool AdminRevoke { get; set; }
    public bool GrantAdminPermissions { get; set; }
    public bool RevokeAdminPermissions { get; set; }
    public bool EditMessages { get; set; }
    public bool DeleteMessages { get; set; }
}
