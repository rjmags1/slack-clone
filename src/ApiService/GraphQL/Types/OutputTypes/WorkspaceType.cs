using System.Collections;
using GraphQL;
using GraphQL.Types;
using PersistenceService.Utils.GraphQL;
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
        Field<WorkspaceMembersConnectionType>("members")
            .Description("The members of the workspace")
            .Argument<UsersFilterInputType>(
                "usersFilter",
                "Filter for resolving workspace members"
            )
            .ResolveAsync(async context =>
            {
                UsersFilter? usersFilter = context.GetArgument<UsersFilter>(
                    "usersFilter"
                );
                if (usersFilter is null)
                {
                    throw new ArgumentNullException(
                        "usersFilter required for workspace members field"
                    );
                }

                FieldInfo fieldInfo = FieldAnalyzer.WorkspaceMembers(context);

                return await data.GetWorkspaceMembers(fieldInfo, usersFilter);
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
