using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using Common.Utils;

namespace Common.SlackCloneGraphQL.Types;

public class DirectMessageGroupType
    : ObjectGraphType<DirectMessageGroup>,
        INodeGraphType<DirectMessageGroup>
{
    public DirectMessageGroupType(ISlackCloneData data)
    {
        Name = "DirectMessageGroup";
        Interface<GroupInterfaceType>();
        Interface<RelayNodeInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the direct message group")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAtUTC")
            .Description("When the direct message group was created")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.CreatedAt
                        : GetSourceFromContext(context)!.CreatedAt
            );
        Field<
            NonNullGraphType<
                ListGraphType<NonNullGraphType<DirectMessageGroupMemberType>>
            >
        >("members")
            .Description("The members of the direct message group")
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Members
                        : GetSourceFromContext(context)!.Members
            );
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
                var dbCols = FieldAnalyzer.DirectMessageDbColumns(
                    GraphQLUtils.GetNodeASTFromConnectionAST(
                        context.FieldAst,
                        context.Document,
                        "DirectMessagesConnection",
                        "DirectMessagesConnectionEdge"
                    ),
                    context.Document
                );

                return await data.GetDirectMessages(
                    context.Source.Id,
                    messagesFilter,
                    first,
                    after,
                    dbCols
                );
            });
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the group")
            .ResolveAsync(async context =>
            {
                //Guid sub = GraphQLUtils.GetSubClaim(
                //(context.UserContext as GraphQLUserContext)!
                //);
                //if (context.Source.Members is null)
                //{
                //return "DEFER_TO_CLIENT";
                //}
                //List<Guid> memberIds = context.Source.Members
                //.Where(member => member.UserId != sub)
                //.Select(member => member.UserId)
                //.ToList();
                //List<string> memberNames = new();
                //foreach (Guid memberId in memberIds)
                //{
                //var user = await data.GetUserById(
                //memberId,
                //new List<string> { "username" }
                //);
                //memberNames.Add(user.Username);
                //}
                //return string.Join(", ", memberNames);
                throw new NotImplementedException();
            });
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description(
                "The workspace associated with the direct message group"
            )
            .Resolve(
                context =>
                    GetSourceFromContext(context) is null
                        ? context.Source.Workspace
                        : GetSourceFromContext(context)!.Workspace
            );
    }

    private DirectMessageGroup? GetSourceFromContext(
        IResolveFieldContext<DirectMessageGroup> context
    )
    {
        return context.UserContext.ContainsKey("source")
            ? (DirectMessageGroup)context.UserContext["source"]!
            : null;
    }
}

public class DirectMessageGroup : INode, IGroup
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable CS8618
    public List<DirectMessageGroupMember> Members { get; set; }
    public Connection<Message> Messages { get; set; }
    public string Name { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
