using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

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
                throw new NotImplementedException();
                //var userId = context.GetArgument<Guid>("id");
                //var fragments = (
                //context.UserContext["fragments"]
                //as Dictionary<string, string>
                //)!;
                //var query = GraphQLUtils.GetQuery(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //var userFieldsInfo = FieldAnalyzer.User(query, fragments);
                //return await data.GetUserById(
                //userId,
                //userFieldsInfo.SubfieldNames
                //);
            });
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .ResolveAsync(async context =>
            {
                var workspaceId = context.GetArgument<Guid>("id");
                return await data.GetWorkspace(workspaceId);
            });
        Field<NonNullGraphType<ChannelsConnectionType>>("channels")
            .Argument<NonNullGraphType<ChannelsFilterInputType>>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                //var first = context.GetArgument<int>("first");
                //var after = context.GetArgument<Guid?>("after");
                //ChannelsFilter channelsFilter =
                //context.GetArgument<ChannelsFilter>("filter");
                //var fragments = (
                //context.UserContext["fragments"]
                //as Dictionary<string, string>
                //)!;
                //var query = GraphQLUtils.GetQuery(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //var channelsFieldInfo = FieldAnalyzer.Channels(
                //query,
                //fragments
                //);

                //return await data.GetChannels(
                //first,
                //after,
                //channelsFilter,
                //channelsFieldInfo
                //);
                throw new NotImplementedException();
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
                //var first = context.GetArgument<int>("first");
                //var after = context.GetArgument<Guid?>("after");
                //DirectMessageGroupsFilter directMessageGroupsFilter =
                //context.GetArgument<DirectMessageGroupsFilter>("filter");
                //var fragments = (
                //context.UserContext["fragments"]
                //as Dictionary<string, string>
                //)!;
                //var query = GraphQLUtils.GetQuery(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //var directMessageGroupsFieldInfo =
                //FieldAnalyzer.DirectMessageGroups(query, fragments);

                //return await data.GetDirectMessageGroups(
                //first,
                //after,
                //directMessageGroupsFilter,
                //directMessageGroupsFieldInfo
                //);
                throw new NotImplementedException();
            });
        Field<NonNullGraphType<StarredConnectionType>>("starred")
            .Argument<NonNullGraphType<StarredFilterInputType>>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                //var first = context.GetArgument<int>("first");
                //var after = context.GetArgument<Guid?>("after");
                //StarredFilter starredFilter =
                //context.GetArgument<StarredFilter>("filter");
                //var fragments = (
                //context.UserContext["fragments"]
                //as Dictionary<string, string>
                //)!;
                //var query = GraphQLUtils.GetQuery(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //var starredFieldInfo = FieldAnalyzer.Starred(query, fragments);

                //return await data.GetStarred(
                //first,
                //after,
                //starredFilter,
                //starredFieldInfo
                //);
                throw new NotImplementedException();
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
