using ApiService.Utils;
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
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .Argument<UsersFilterInputType>(
                "filter",
                "Filter for resolving workspace members"
            )
            .ResolveAsync(async context =>
            {
                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                UsersFilter? usersFilter =
                    context.GetArgument<UsersFilter>("filter")
                    ?? throw new ArgumentNullException(
                        "usersFilter required for workspace members field"
                    );
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                FieldInfo fieldInfo = FieldAnalyzer.WorkspaceMembers(
                    query,
                    fragments
                );

                return await data.GetWorkspaceMembers(
                    fieldInfo,
                    usersFilter,
                    first,
                    after
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
