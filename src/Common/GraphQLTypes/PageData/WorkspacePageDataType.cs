using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using Common.Utils;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspacePageDataType : ObjectGraphType<WorkspacePageData>
{
    public WorkspacePageDataType(ISlackCloneData data)
    {
        Name = "WorkspacePageData";
        Interface<RelayNodeInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description(
                "A UUID for page data - included for Relay compatibility"
            )
            .Resolve(context => context.Source.Id ?? Guid.NewGuid());
        Field<NonNullGraphType<UserType>>("user")
            .Description("The authenticated user viewing the workspace.")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("id");
                var dbCols = FieldAnalyzer.UserDbColumns(
                    context.FieldAst,
                    context.Document
                );
                return await data.GetUserById(userId, dbCols);
            });
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .ResolveAsync(async context =>
            {
                var workspaceId = context.GetArgument<Guid>("id");
                var dbCols = FieldAnalyzer.WorkspaceDbColumns(
                    context.FieldAst,
                    context.Document
                );
                return await data.GetWorkspace(workspaceId, dbCols);
            });
        Field<NonNullGraphType<ChannelsConnectionType>>("channels")
            .Argument<NonNullGraphType<ChannelsFilterInputType>>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                ChannelsFilter channelsFilter =
                    context.GetArgument<ChannelsFilter>("filter");

                var dbColumns = FieldAnalyzer.WorkspaceDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "ChannelsConnection",
                        "ChannelsConnectionEdge"
                    ),
                    context.Document
                );
                return await data.GetChannels(
                    first,
                    after,
                    channelsFilter,
                    dbColumns
                );
            });
        Field<NonNullGraphType<DirectMessageGroupsConnectionType>>(
                "directMessageGroups"
            )
            .Argument<NonNullGraphType<DirectMessageGroupsFilterInputType>>(
                "filter"
            )
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                var dbCols = FieldAnalyzer.DirectMessageGroupDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "DirectMessageGroupsConnection",
                        "DirectMessageGroupsConnectionEdge"
                    ),
                    context.Document
                );

                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                DirectMessageGroupsFilter directMessageGroupsFilter =
                    context.GetArgument<DirectMessageGroupsFilter>("filter");
                var fragments = (
                    context.UserContext["fragments"]
                    as Dictionary<string, string>
                )!;

                return await data.GetDirectMessageGroups(
                    first,
                    after,
                    directMessageGroupsFilter,
                    dbCols
                );
            });
        Field<NonNullGraphType<StarredConnectionType>>("starred")
            .Argument<NonNullGraphType<StarredFilterInputType>>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                var dbCols = FieldAnalyzer.GroupDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "StarredConnection",
                        "StarredConnectionEdge"
                    ),
                    context.Document
                );

                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                StarredFilter starredFilter =
                    context.GetArgument<StarredFilter>("filter");

                return await data.GetStarred(
                    first,
                    after,
                    starredFilter,
                    dbCols
                );
            });
    }
}

public class WorkspacePageData
{
    public Guid? Id { get; set; }
#pragma warning disable CS8618
    public User User { get; set; }
    public Workspace Workspace { get; set; }
    public Connection<Channel> Channels { get; set; }
    public Connection<DirectMessageGroup> DirectMessageGroups { get; set; }
    public Connection<Message> Starred { get; set; }
#pragma warning restore CS8618
}
