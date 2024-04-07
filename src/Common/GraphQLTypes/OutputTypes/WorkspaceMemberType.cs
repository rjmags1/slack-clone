using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspaceMemberType
    : ObjectGraphType<WorkspaceMember>,
        INodeGraphType<WorkspaceMember>
{
    public WorkspaceMemberType()
    {
        Name = "WorkspaceMember";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("Id of the workspace membership")
            .Resolve(context => context.Source.Id);
        Field<FileType>("avatar")
            .Description("User avatar specific to this workspace")
            .Resolve(context => context.Source.Avatar);
        Field<NonNullGraphType<DateTimeGraphType>>("joinedAt")
            .Description("When the user joined the workspace")
            .Resolve(context => context.Source.JoinedAt);
        Field<NonNullGraphType<StringGraphType>>("title")
            .Description("The title the user has within the workspace")
            .Resolve(context => context.Source.Title);
        Field<NonNullGraphType<UserType>>("user")
            .Description("The user associated with the membership")
            .Resolve(context => context.Source.User);
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace associated with the membership")
            .Resolve(context => context.Source.Workspace);
        Field<WorkspaceMemberInfoType>("workspaceMemberInfo")
            .Description("Info about the user's workspace membership")
            .Resolve(context => context.Source.WorkspaceMemberInfo);
    }
}

public class WorkspaceMember : INode
{
    public Guid Id { get; set; }
    public DateTime JoinedAt { get; set; }
#pragma warning disable CS8618
    public File? Avatar { get; set; }
    public string Title { get; set; }
    public User User { get; set; }
    public Workspace Workspace { get; set; }
    public WorkspaceMemberInfo? WorkspaceMemberInfo { get; set; }
#pragma warning restore CS8618
}
