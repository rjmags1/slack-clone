using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using Common.Utils;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspaceType
    : ObjectGraphType<Workspace>,
        INodeGraphType<Workspace>
{
    public WorkspaceType(ISlackCloneData data)
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
                UsersFilter? usersFilter = context.GetArgument<UsersFilter>(
                    "filter"
                );

                var dbCols = FieldAnalyzer.ChannelMemberDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "WorkspaceMembersConnection",
                        "WorkspaceMembersConnectionEdge"
                    ),
                    context.Document
                );

                return await data.GetWorkspaceMembers(
                    context.Source.Id,
                    usersFilter,
                    first,
                    after,
                    dbCols
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
