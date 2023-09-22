using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using PersistenceService.Utils.GraphQL;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class DirectMessageGroupType
    : ObjectGraphType<DirectMessageGroup>,
        INodeGraphType<DirectMessageGroup>
{
    public DirectMessageGroupType(SlackCloneData data)
    {
        Name = "DirectMessageGroup";
        Interface<GroupInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the direct message group")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAtUTC")
            .Description("When the direct message group was created")
            .Resolve(context => context.Source.CreatedAt);
        Field<
            NonNullGraphType<
                ListGraphType<NonNullGraphType<DirectMessageGroupMemberType>>
            >
        >("members")
            .Description("The members of the direct message group")
            .Resolve(context => context.Source.Members);
        Field<NonNullGraphType<DirectMessagesConnectionType>>("messages")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .Argument<MessagesFilterInputType>("filter")
            .Description(
                "Relay connection representing messages in the direct message group conversation"
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

                return await data.GetDirectMessages(
                    sub,
                    context.Source.Id,
                    fieldInfo,
                    messagesFilter,
                    first,
                    after
                );
            });
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description(
                "The workspace associated with the direct message group"
            )
            .Resolve(context => context.Source.Workspace);
    }
}

public class DirectMessageGroup : INode, IGroup
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable CS8618
    public List<DirectMessageGroupMember> Members { get; set; }
    public Connection<Message> Messages { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
