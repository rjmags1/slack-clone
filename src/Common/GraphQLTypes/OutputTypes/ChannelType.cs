using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using Common.Utils;

namespace Common.SlackCloneGraphQL.Types;

public class ChannelType : ObjectGraphType<Channel>, INodeGraphType<Channel>
{
    public ChannelType(ISlackCloneData data)
    {
        Name = "Channel";
        Interface<GroupInterfaceType>();
        Interface<RelayNodeInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the channel.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<BooleanGraphType>>("allowThreads")
            .Description("Whether threads are allowed in the channel.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.AllowThreads
                        : GetSourceFromContext(context)!.AllowThreads
            );
        Field<NonNullGraphType<IntGraphType>>("allowedPostersMask")
            .Description(
                "Bitmask representing who is allowed to post in the channel."
            )
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.AllowedPostersMask
                        : GetSourceFromContext(context)!.AllowedPostersMask
            );
        Field<NonNullGraphType<FileType>>("avatar")
            .Description("The avatar of the channel.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Avatar
                        : GetSourceFromContext(context)!.Avatar
            );
        Field<NonNullGraphType<DateTimeGraphType>>("createdAtUTC")
            .Description("When the channel was created.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.CreatedAt
                        : GetSourceFromContext(context)!.CreatedAt
            );
        Field<UserType>("createdBy")
            .Description("Who created the channel.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.CreatedBy
                        : GetSourceFromContext(context)!.CreatedBy
            );
        Field<StringGraphType>("description")
            .Description("A brief description of the channel.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Description
                        : GetSourceFromContext(context)!.Description
            );
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
                var dbCols = FieldAnalyzer.ChannelMemberDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "ChannelMembersConnection",
                        "ChannelMembersConnectionEdge"
                    ),
                    context.Document
                );

                return await data.GetChannelMembers(
                    context.Source.Id,
                    usersFilter,
                    first,
                    after,
                    dbCols
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
                var dbCols = FieldAnalyzer.ChannelMessageDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "ChannelMessagesConnection",
                        "ChannelMessagesConnectionEdge"
                    ),
                    context.Document
                );

                return await data.GetChannelMessages(
                    context.Source.Id,
                    messagesFilter,
                    first,
                    after,
                    dbCols
                );
            });
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the channel.")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Name
                        : GetSourceFromContext(context)!.Name
            );
        Field<NonNullGraphType<IntGraphType>>("numMembers")
            .Description("The number of members of the channel")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.NumMembers
                        : GetSourceFromContext(context)!.NumMembers
            );
        Field<NonNullGraphType<BooleanGraphType>>("private")
            .Description(
                "Whether viewing the channel is restricted to certain workspace members or not."
            )
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Private
                        : GetSourceFromContext(context)!.Private
            );
        Field<StringGraphType>("topic")
            .Description("The topic of the channel")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Topic
                        : GetSourceFromContext(context)!.Topic
            );
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace containing the channel")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Workspace
                        : GetSourceFromContext(context)!.Workspace
            );
    }

    private Channel? GetSourceFromContext(IResolveFieldContext<Channel> context)
    {
        return context.UserContext.ContainsKey("source")
            ? (Channel)context.UserContext["source"]!
            : null;
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
