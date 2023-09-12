using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using PersistenceService.Utils.GraphQL;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class ChannelType : ObjectGraphType<Channel>, INodeGraphType<Channel>
{
    public ChannelType(SlackCloneData data)
    {
        Name = "Channel";
        Interface<GroupInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the channel.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<BooleanGraphType>>("allowThreads")
            .Description("Whether threads are allowed in the channel.")
            .Resolve(context => context.Source.AllowThreads);
        Field<NonNullGraphType<IntGraphType>>("allowedPostersMask")
            .Description(
                "Bitmask representing who is allowed to post in the channel."
            )
            .Resolve(context => context.Source.AllowedPostersMask);
        Field<NonNullGraphType<FileType>>("avatar")
            .Description("The avatar of the channel.")
            .Resolve(context => context.Source.Avatar);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the channel was created.")
            .Resolve(context => context.Source.CreatedAt);
        Field<UserType>("createdBy")
            .Description("Who created the channel.")
            .Resolve(context => context.Source.CreatedBy);
        Field<StringGraphType>("description")
            .Description("A brief description of the channel.")
            .Resolve(context => context.Source.Description);
        Field<NonNullGraphType<ChannelMembersConnectionType>>("members")
            .Description(
                "Relay connection representing collection of channel members."
            )
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .Argument<NonNullGraphType<UsersFilterInputType>>("filter")
            .ResolveAsync(async context =>
            {
                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                UsersFilter usersFilter = context.GetArgument<UsersFilter>(
                    "filter"
                );
                if (usersFilter.Channels is not null)
                {
                    throw new InvalidOperationException();
                }
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                FieldInfo fieldInfo = FieldAnalyzer.ChannelMembers(
                    query,
                    fragments
                );

                return await data.GetChannelMembers(
                    fieldInfo,
                    context.Source.Id,
                    usersFilter,
                    first,
                    after
                );
            });
        Field<NonNullGraphType<ChannelMessagesConnectionType>>("messages")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .Argument<MessagesFilterInputType>("filter")
            .Description(
                "Relay connection representing collection of channel messages."
            )
            .ResolveAsync(async context =>
            {
                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                MessagesFilter? messagesFilter =
                    context.GetArgument<MessagesFilter>("filter");
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                FieldInfo fieldInfo = FieldAnalyzer.ChannelMessages(
                    query,
                    fragments
                );
                Guid sub = GraphQLUtils.GetSubClaim(
                    (context.UserContext as GraphQLUserContext)!
                );

                return await data.GetChannelMessages(
                    sub,
                    context.Source.Id,
                    fieldInfo,
                    messagesFilter,
                    first,
                    after
                );
            });
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the channel.")
            .Resolve(context => context.Source.Name);
        Field<NonNullGraphType<IntGraphType>>("numMembers")
            .Description("The number of members of the channel")
            .Resolve(context => context.Source.NumMembers);
        Field<NonNullGraphType<BooleanGraphType>>("private")
            .Description(
                "Whether viewing the channel is restricted to certain workspace members or not."
            )
            .Resolve(context => context.Source.Private);
        Field<StringGraphType>("topic")
            .Description("The topic of the channel")
            .Resolve(context => context.Source.Topic);
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace containing the channel")
            .Resolve(context => context.Source.Workspace);
    }
}

public class Channel : INode, IGroup
{
    public Guid Id { get; set; }
    public bool AllowThreads { get; set; }
    public int AllowedPostersMask { get; set; }
#pragma warning disable CS8618
    public File Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? CreatedBy { get; set; }
    public string? Description { get; set; }
    public Connection<ChannelMember> Members { get; set; }
    public Connection<Message> Messages { get; set; }
    public string Name { get; set; }
    public int NumMembers { get; set; }
    public bool Private { get; set; }
    public string? Topic { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
