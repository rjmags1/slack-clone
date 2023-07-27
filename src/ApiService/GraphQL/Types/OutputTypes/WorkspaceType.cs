using System.Collections;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class WorkspaceType
    : ObjectGraphType<Workspace>,
        INodeGraphType<Workspace>
{
    public WorkspaceType(SlackCloneData data)
    {
        Name = "Workspace";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("Id of the workspace")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("Date and time of workspace creation")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<StringGraphType>>("description")
            .Description("The description of the workspace")
            .Resolve(context => context.Source.Description);
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the workspace")
            .Resolve(context => context.Source.Name);
        Field<NonNullGraphType<FileType>>("avatar")
            .Description("The avatar for the workspace")
            .Resolve(context => context.Source.Avatar);
        Field<NonNullGraphType<WorkspaceMembersConnectionType>>("members")
            .Description("The members of the workspace")
            .ResolveAsync(async context =>
            {
                UsersFilter usersFilter = context.GetArgument<UsersFilter>(
                    "usersFilter"
                );

                (string, ArrayList?) requestedFields =
                    FieldAnalyzer.WorkspaceMembers(context);

                return await data.GetWorkspaceMembers(
                    requestedFields,
                    usersFilter
                );
            });
        Field<NonNullGraphType<IntGraphType>>("numMembers")
            .Description("Members in the workspace")
            .Resolve(context => context.Source.NumMembers);
    }
}

#pragma warning disable CS8618
public class Workspace : INode
{
    public Guid Id { get; set; }
    public File Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public int NumMembers { get; set; }
    public Connection<WorkspaceMember> Members { get; set; }
}
