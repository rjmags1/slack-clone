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
        Field<ThemeType>("theme")
            .Description("Theme for this workspace")
            .Resolve(context => context.Source.Theme);
    }
}

public class WorkspaceMemberInfo
{
    public bool Admin { get; set; }
    public bool Owner { get; set; }
#pragma warning disable CS8618
    public WorkspaceAdminPermissions? WorkspaceAdminPermissions { get; set; }
    public Theme Theme { get; set; }
#pragma warning restore CS8618
}
