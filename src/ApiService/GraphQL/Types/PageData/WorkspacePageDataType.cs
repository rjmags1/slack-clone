using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class WorkspacePageDataType : ObjectGraphType<WorkspacePageData>
{
    public WorkspacePageDataType(SlackCloneData data)
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
                var fragments = (
                    context.UserContext["fragments"]
                    as Dictionary<string, string>
                )!;
                var query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var userFieldsInfo = FieldAnalyzer.User(query, fragments);
                return await data.GetUserById(
                    userId,
                    userFieldsInfo.SubfieldNames
                );
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
            .Resolve(context =>
            {
                throw new NotImplementedException();
            });
        Field<NonNullGraphType<DirectMessageGroupsConnectionType>>(
                "directMessageGroups"
            )
            .Argument<NonNullGraphType<DirectMessageGroupsFilterInputType>>(
                "filter"
            )
            .Resolve(context =>
            {
                throw new NotImplementedException();
            });
        Field<NonNullGraphType<StarredConnectionType>>("starred")
            .Argument<NonNullGraphType<StarredFilterInputType>>("filter")
            .Resolve(context =>
            {
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
