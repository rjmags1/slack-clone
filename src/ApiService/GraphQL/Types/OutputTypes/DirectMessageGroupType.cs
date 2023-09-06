using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class DirectMessageGroupType
    : ObjectGraphType<DirectMessageGroup>,
        INodeGraphType<DirectMessageGroup>
{
    public DirectMessageGroupType()
    {
        Name = "DirectMessageGroup";
        Interface<GroupInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the direct message group")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the direct message group was created")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<UserType>>>>(
                "members"
            )
            .Description("The members of the direct message group")
            .Resolve(context => throw new NotImplementedException());
        Field<NonNullGraphType<DirectMessagesConnectionType>>("messages")
            .Description(
                "Relay connection representing messages in the direct message group conversation"
            )
            .Resolve(context => throw new NotImplementedException());
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the direct message group")
            .Resolve(context => throw new NotImplementedException()); // names of members
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
    public List<User> Members { get; set; }
    public Connection<Message> Messages { get; set; }
    public string Name { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
